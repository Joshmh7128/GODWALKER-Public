Shader "Custom/Grass"
{
    Properties
    {
        _HeightMap("_HeightMap", 2D) = "white" {}
        _HeightMapTile("_HeightMapTile", Float) = 0.01
        _BaseColorTexture("_BaseColorTexture", 2D) = "white" {}
        _BaseColorTextureTile("_BaseColorTextureTile", Float) = 0.01

        [Header(Grass Shape)]
        _GrassWidth("_GrassWidth", Float) = 1
        _GrassHeight("_GrassHeight", Float) = 1
        _MinFlattenAngle("_MinFlattenAngle", Float) = 45
        _FlattenAmount("_FlattenAmount", Float) = 30

        [Header(Wind)]
        _WindAIntensity("_WindAIntensity", Float) = 1.77
        _WindAFrequency("_WindAFrequency", Float) = 4
        _WindATiling("_WindATiling", Vector) = (0.1,0.1,0)
        _WindAWrap("_WindAWrap", Vector) = (0.5,0.5,0)

        _WindBIntensity("_WindBIntensity", Float) = 0.25
        _WindBFrequency("_WindBFrequency", Float) = 7.7
        _WindBTiling("_WindBTiling", Vector) = (.37,3,0)
        _WindBWrap("_WindBWrap", Vector) = (0.5,0.5,0)

        [Header(Lighting)]
        _RandomNormal("_RandomNormal", Float) = 0.15

        //make SRP batcher happy
        //[HideInInspector]_PivotPosWS("_PivotPosWS", Vector) = (0,0,0,0)
        //[HideInInspector]_BoundSize("_BoundSize", Vector) = (1,1,0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Cull Back //use default culling because this shader is billboard 
            ZTest Less
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------
            // Universal Render Pipeline keywords
            // When doing custom shaders you most often want to copy and paste these #pragmas
            // These multi_compile variants are stripped from the build depending on:
            // 1) Settings in the URP Asset assigned in the GraphicsSettings at build time
            // e.g If you disabled AdditionalLights in the asset then all _ADDITIONA_LIGHTS variants
            // will be stripped from build
            // 2) Invalid combinations are stripped. e.g variants with _MAIN_LIGHT_SHADOWS_CASCADE
            // but not _MAIN_LIGHT_SHADOWS are invalid and therefore stripped.
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            // -------------------------------------

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct v2f
            {
                float4 pos : SV_POSITION;
                half3 color : COLOR0;
			};

            CBUFFER_START(UnityPerMaterial)
                float3 _MeshPosition; // Set from script

                float _GrassWidth;
                float _GrassHeight;

                // Buffer of randomly generated grass positions (local to parent object)
                StructuredBuffer<float3> _Positions; // Set from script

                // Buffer of normals (one for each triangle)
                StructuredBuffer<float3> _Normals; // Set from script

                // Right now these are set from script - they could be made into properties and set in the material instead
                float3 _ColorA;
                float3 _ColorB;

                sampler2D _HeightMap;

                int _NormalsLength; // Set from script
            CBUFFER_END

            sampler2D _BaseColorTexture;
            float _BaseColorTextureTile;

            float _MinFlattenAngle;
            float _FlattenAmount;

            float _HeightMapTile;

            float _WindAIntensity;
            float _WindAFrequency;
            float2 _WindATiling;
            float2 _WindAWrap;

            float _WindBIntensity;
            float _WindBFrequency;
            float2 _WindBTiling;
            float2 _WindBWrap;

            half _RandomNormal;

             // Set from script -- _Angle is camera angle and _CameraForward is camera forward direction
            float2 _Angle;
            float3 _CameraForward;

            half3 ApplySingleDirectLight(Light light, half3 N, half3 V, half3 albedo, half positionOSY)
            {
                half3 H = normalize(light.direction + V);

                //direct diffuse 
                half directDiffuse = dot(N, light.direction) * 0.5 + 0.5; //half lambert, to fake grass SSS

                //direct specular
                float directSpecular = saturate(dot(N,H));
                //pow(directSpecular,8)
                directSpecular *= directSpecular;
                directSpecular *= directSpecular;
                directSpecular *= directSpecular;
                //directSpecular *= directSpecular; //enable this line = change to pow(directSpecular,16)

                //add direct directSpecular to result
                directSpecular *= 0.1 * positionOSY;//only apply directSpecular to grass's top area, to simulate grass AO

                half3 lighting = light.color * (light.shadowAttenuation * light.distanceAttenuation);
                half3 result = (albedo * directDiffuse + directSpecular) * lighting;
                return result; 
            }

            // Pseudo random - we can pass this an index (or vertex_id!) to get back a very different number between 0 and 1
            float hash(uint state)
            {
                state ^= 2747636419u;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                return state / 4294967295.0;
            }

            float3 RotateAroundYInDegrees(float3 vertex, float degrees)
            {
                float alpha = degrees * 0.0174532924;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m,vertex.xz), vertex.y).xzy;
            }

            float3 RotateAroundXInDegrees(float3 vertex, float degrees)
            {
                float alpha = degrees * 0.0174532924;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m,vertex.yz), vertex.x).xyz;
            }

            // This is called for every vertex this shader is creating (3 times for each triangle)
            // vertex_id is the vertex index, instance_id is the triangle index
            v2f vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
            {
                // Don't render this blade if its ground normal is too steep
                // (checking dot product between global up and triangle normal and comparing to ~45 degrees - this could be turned into an inspector variable)
                if (dot(float3(0, 1, 0), _Normals[instance_id % _NormalsLength]) < 0.525)
                {
                    v2f none;
                    return none;
				}
                
                // Get corresponding random local position using our triangle index
                float3 localPosition = _Positions[instance_id];

                // Calculate blade height from height map (only the red channel - could later pack more grass info into the other channels if needed)
                float height = tex2Dlod(_HeightMap, float4(localPosition.xz * _HeightMapTile,0,0)).x;

                // Don't render blade if height is too low
                if (height < 0.03)
                {
                    v2f none;
                    return none;
				}

                height *= _GrassHeight;

                // Add a bit of randomness to the height to make the grass less uniform
                height *= lerp(0.8, 1.1, hash(abs(localPosition.z) + abs(localPosition.x) * 2123));

                // If vertex position is 0 this is the bottom left vertex. Bottom right if 1. Top i 2.
                // Getting position from a const array COULD be a bit faster? idk
                float3 vertexPosition = vertex_id == 0 ? float3(-1 * _GrassWidth, 0, 0) : vertex_id == 1 ? float3(1 * _GrassWidth, 0, 0) : float3(0, height, 0);

                // Cbeck camera angle (_Angle) to see if we need to flatten grass (need some way to get camera angle without passing it from C# so that this works for multiple cameras)
                if (_Angle.x < _MinFlattenAngle || _Angle.x > 90)
                {
                    // Don't flatten
                    _Angle.x = 0;
				}
                else
                {
                    // Flatten if angle is between _MinFlattenAngle and 90
                    _Angle.x = _FlattenAmount * ((_Angle.x - _MinFlattenAngle) / (90 - _MinFlattenAngle));
				}
                
                // Rotate vertex around x axis based on camera angle - this is so that the blades don't disappear when looking at them from above
                vertexPosition = RotateAroundXInDegrees(vertexPosition, 90 - _Angle.x);
                
                // Rotate vertex around y axis so that the grass blade always faces the camera
                float cameraYAngle = 90 - _Angle.y;
                vertexPosition = RotateAroundYInDegrees(vertexPosition, cameraYAngle);

                // Add vertex position and localPosition (to get position in object space) and then add the parent mesh position (to get position in world space)
                float3 position = vertexPosition + localPosition + _MeshPosition;
                
                // Calculate wind offset by sampling sin waves at two different values (for me wind.x is low frequency & high intensity and wind.y is high frequency & low intensity)
                // Multiply wind by vertexPosition.y so that it only affects the top of the grass
                float2 wind;
                wind.x = (sin(_Time.y * _WindAFrequency + localPosition.x * _WindATiling.x + localPosition.z * _WindATiling.y)*_WindAWrap.x+_WindAWrap.y) * _WindAIntensity * vertexPosition.y;
                wind.y = (sin(_Time.y * _WindBFrequency + localPosition.x * _WindBTiling.x + localPosition.z * _WindBTiling.y)*_WindBWrap.x+_WindBWrap.y) * _WindBIntensity * vertexPosition.y;

                // Rotate wind in direction of camera y - otherwise the grass gets rotated weirdly with wind
                // You'd think it'd look weird when the wind rotates with the camera, but it just doesn't :o
                float rotationAngleRad = (cameraYAngle) * 0.0174532924;
                float rotationAngleCos = cos(rotationAngleRad);
                float rotationAngleSin = sin(rotationAngleRad);
                position.x += rotationAngleCos * wind.x;
                position.z += rotationAngleSin * wind.x;
                position.z += rotationAngleCos * wind.y;
                position.x += rotationAngleSin * wind.y;

                v2f o;

                // Transform position into clip space so unity can do it's thing (positionCS)
                o.pos = mul(UNITY_MATRIX_VP, float4(position, 1));

                // Get view vectors for lighting
                float3 viewWS = _WorldSpaceCameraPos - _MeshPosition;
                float ViewWSLength = length(viewWS);

                //lighting data
                Light mainLight;
#if _MAIN_LIGHT_SHADOWS
                mainLight = GetMainLight(TransformWorldToShadowCoord(position));
#else
                mainLight = GetMainLight();
#endif
                // We want some randomness in each grass blade's normal - it makes the grass look less uniform and less shit
                half3 randomAddToN = (_RandomNormal* sin(localPosition.x * 82.32523 + localPosition.z) + float3(wind.x, 0, wind.y) * -0.25);

                // Set normal to the camera forward z plus our random normal (I don't even remeber why I did just the camera z so maybe this could be improved idk)
                half3 N = half3(0, 0, _CameraForward.z) + randomAddToN * 0.5;

                half3 V = viewWS / ViewWSLength;

                // I'm setting the grass color from _ColorA and _ColorB that are sent to the shader from C# and blending between them based on a texture (perlin is good for this)
                // This could easily be changed - could could be sampled from color texture or an array of colors could be sent from C# so it can be per-triangle
                float colorPercentage = tex2Dlod(_BaseColorTexture, float4(localPosition.xz * _BaseColorTextureTile,0,0)).x;
                half3 albedo = lerp(_ColorA, _ColorB, colorPercentage);

                // Darken color on bottom vertices to give the grass a kind of fake self-shadow
                if (vertex_id < 2)
                {
                    albedo *= 0.35;
				}
                
                half3 lightingResult = ApplySingleDirectLight(mainLight, N, V, albedo, vertexPosition.y);

#if _ADDITIONAL_LIGHTS

                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    // Similar to GetMainLight, but it takes a for-loop index. This figures out the
                    // per-object light index and samples the light buffer accordingly to initialized the
                    // Light struct. If _ADDITIONAL_LIGHT_SHADOWS is defined it will also compute shadows.
                    Light light = GetAdditionalLight(i, position);
                
                    // Same functions used to shade the main light.
                    lightingResult += ApplySingleDirectLight(light, N, V, albedo, vertexPosition.y);
                }
#endif
                o.color = lightingResult;
                return o;
			}

            half3 frag(v2f i) : SV_Target
            {
                // You could probably apply a texture to the grass here
                return i.color;
            }
            ENDHLSL
        }
    }
}