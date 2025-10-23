/*
{
    "CATEGORIES": [
        "Automatically Converted",
        "Shadertoy"
    ],
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/clKSzz by Sakari369.  Ancient Aztec psychedelic visuals.\nThe ceremonies have began.",
    "IMPORTED": {
    },
    "INPUTS": [
    ]
}

*/


// Ancient Aztec Visuals
// Watch in fullscreen.
//
// Based on https://www.shadertoy.com/view/clGXzR.
//
// The study of sacred geometry helps one to see the lines between the lines.
//
// -- Sakari @ OmniGeometry.com

vec3 pal(float t) {
    vec3 a = vec3(0.5, 0.5, 0.5);
    vec3 b = vec3(0.5, 0.5, 0.5);
    vec3 c = vec3(1.0, 1.0, 1.0);

    vec3 d = vec3(0.269,0.410,0.587);

    return a + b*cos( 6.28318*(c*t+d) );
}

void main() {

    vec2 uv = (gl_FragCoord.xy * 2.0 - RENDERSIZE.xy) / RENDERSIZE.y;
    vec2 uv_0 = uv;
    float it = 1.35 * TIME;
    vec3 outColor = vec3(0.0);    
    
    for (float i = 0.0; i < 2.0; i++) {
        vec3 color = pal(length(0.333*uv_0 + 0.01*it) + 0.001*i + 0.16*it);
        uv = fract(1.618*uv) - 0.5;
        float d = 0.933*length(uv) * 1.33*exp(-length(uv_0));
        d = sin(26.0*d + it) / 28.0;
        d = abs(d);
        d = pow(0.005 / d, 1.2);
        outColor += color * d;
    }
        
    gl_FragColor = vec4(outColor, 1.0);
}
