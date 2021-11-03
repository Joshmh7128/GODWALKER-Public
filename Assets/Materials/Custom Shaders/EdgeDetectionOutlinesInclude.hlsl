#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

// the sobel effect runs by sampling the texture around a point to see if there are any
// large changes in the depth. Each sample is multiplied by a convolution matrix weight for
// the x and y components seperately. Each value is then added together, and the final sobel
// value is the length of the resulting float2. Higher values mean the algorithm detected more 
// of an edge, and will be able to display it

// these are points to sample relative to the starting point
static float2 sobelSamplePoints[9] = {
	float2(-1,1), float2(0,1), float2(1,1),
	float2(-1,0), float2(0,0), float2(1,0),
	float2(-1,-1), float2(0,-1), float2(1,-1),
};

// weights for the x component
static float sobelXMatrix[9] = {
	1,0,-1,
	2,0,-2,
	1,0,-1
};

// weights for the y component
static float sobelYMatrix[9] = {
	1,2,1,
	0,0,0,
	-1,-2,-1
};

// this function runs the sobel algorithm over the depth texture
void DepthSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobel = 0;
	// we can unroll this to make it more efficient, since it always is a for loop of 9
	[unroll] for (int i = 0; i < 9; i++) {
		float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
		sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}
	// get the final sobel value
	Out = length(sobel);
}

// this function runs the sobel algorithm over the opaque texture
void ColorSobel_float(float2 UV, float Thickness, out float Out)
{	// run three sobel vectors
	float2 sobelR = 0;
	float2 sobelG = 0;
	float2 sobelB = 0;

	[unroll] for (int i = 0; i < 9; i++)
	{
		float3 rgb = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV + sobelSamplePoints[i] * Thickness);
		float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
		// accumulate samples for each color
		sobelR += rgb.r * kernel;
		sobelG += rgb.g * kernel;
		sobelB += rgb.b * kernel;
	}
	// get the final sobel value
	Out = max(length(sobelR), max(length(sobelG), length(sobelB)))
		;
}

#endif