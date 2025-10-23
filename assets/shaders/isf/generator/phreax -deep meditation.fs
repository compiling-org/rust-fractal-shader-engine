/*
{
    "CATEGORIES": [
        "Fractal",
        "3D",
        "Animated",
        "Abstract",
        "Psychedelic",
        "SDF"
    ],
    "DESCRIPTION": "An advanced ISF conversion of a complex Shadertoy fractal, blending SDF-based geometry with intricate repeating patterns and dynamic kaleidoscopic effects. Features comprehensive controls for fractal structure, multiple psychedelic color palettes, animation, camera, and post-processing effects like glitch, shake, brightness, saturation, and contrast.",
    "CREDIT": "Original shader by phreax (2022) - Creative Commons Licence Attribution-NonCommercial-ShareAlike. Converted and enhanced for ISF by Gemini.",
    "ISF_VERSION": "2.0",
    "INPUTS": [
        { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Overall animation speed." },
        { "NAME": "zoom", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Camera zoom level. Higher values zoom in." },
        { "NAME": "xy_control_x", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "X-component for camera rotation/offset (normalized 0-1, maps to 0-2PI)." },
        { "NAME": "xy_control_y", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Y-component for camera rotation/offset (normalized 0-1, maps to 0-PI)." },
        { "NAME": "raymarch_steps", "TYPE": "float", "DEFAULT": 80.0, "MIN": 20.0, "MAX": 200.0, "STEP": 1.0, "DESCRIPTION": "Number of raymarching steps for primary fractal. Higher values increase detail but reduce performance." },
        { "NAME": "max_ray_distance", "TYPE": "float", "DEFAULT": 7.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Maximum distance the ray will travel before stopping." },
        { "NAME": "min_step_distance", "TYPE": "float", "DEFAULT": 0.006, "MIN": 0.0001, "MAX": 0.01, "DESCRIPTION": "Minimum step distance to consider a hit in the raymarcher." },
        { "NAME": "kalei_morph_speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Speed of morphing in the Kalei fractal section." },
        { "NAME": "kalei_z_factor", "TYPE": "float", "DEFAULT": 0.14, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Z-axis factor in Kalei fractal rotation." },
        { "NAME": "kalei_y_factor", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5, "DESCRIPTION": "Y-axis factor in Kalei fractal vertical displacement." },
        { "NAME": "mesh_density_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for the mesh density in the main fractal." },
        { "NAME": "layer_distance", "TYPE": "float", "DEFAULT": 6.5, "MIN": 1.0, "MAX": 15.0, "DESCRIPTION": "Distance between repeating layers in the fractal." },
        { "NAME": "line_width", "TYPE": "float", "DEFAULT": 0.0001, "MIN": 0.00001, "MAX": 0.001, "DESCRIPTION": "Base line width for fractal mesh." },
        { "NAME": "body_pill_radius", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Base radius for the 'body' SDF pill." },
        { "NAME": "body_pill_height_morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Morph factor for the body pill's height/shape." },
        { "NAME": "head_radius", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 1.0, "DESCRIPTION": "Radius of the 'head' sphere." },
        { "NAME": "smooth_blend_fractal_body", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.01, "MAX": 2.0, "DESCRIPTION": "Smooth blend factor between the fractal mesh and the 'body' shape." },
        { "NAME": "star_pattern_width_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for the width of lines in the star pattern background." },
        { "NAME": "star_pattern_density", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Density of the star pattern background." },
        { "NAME": "star_pattern_mod_alpha_blend", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blend between fixed and dynamic alpha for star pattern." },
        { "NAME": "ring_tri_size", "TYPE": "float", "DEFAULT": 0.286, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Size of triangular rings." },
        { "NAME": "ring_hex_size", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Size of hexagonal rings." },
        { "NAME": "ring_circle_size", "TYPE": "float", "DEFAULT": 0.143, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Size of circular rings." },
        { "NAME": "ring_line_width_mult", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Multiplier for line width of rings." },
        { "NAME": "ring_blur", "TYPE": "float", "DEFAULT": 0.001, "MIN": 0.0001, "MAX": 0.01, "DESCRIPTION": "Blur amount for ring lines." },
        { "NAME": "ring_glow_intensity", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of glow for rings." },
        { "NAME": "ring_animation_rate", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Animation speed for rings." },
        { "NAME": "color_palette_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 7.0, "STEP": 1.0, "DESCRIPTION": "Selects one of 8 psychedelic color palettes." },
        { "NAME": "fractal_color_intensity", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of the primary fractal's color." },
        { "NAME": "background_color_mix", "TYPE": "float", "DEFAULT": 0.3, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for background solid color (0=less, 1=more)." },
        { "NAME": "star_color_mix", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Mix factor for star pattern color." },
        { "NAME": "star_hue_intensity", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Intensity of hue shift in star pattern color." },
        { "NAME": "star_pulse_intensity", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of pulsing in star pattern color." },
        { "NAME": "rings_color_mix", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Mix factor for overall rings color." },
        { "NAME": "rings_pulsing_mix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Mix factor for pulsating effect on rings." },
        { "NAME": "chakras_intensity", "TYPE": "float", "DEFAULT": 1.5, "MIN": 0.0, "MAX": 3.0, "DESCRIPTION": "Intensity of the chakra-like background elements." },
        { "NAME": "glitch_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of glitch effect." },
        { "NAME": "glitch_frequency", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of glitch disruptions." },
        { "NAME": "shake_strength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of camera shake effect." },
        { "NAME": "shake_frequency", "TYPE": "float", "DEFAULT": 20.0, "MIN": 0.1, "MAX": 50.0, "DESCRIPTION": "Frequency of camera shake oscillations." },
        { "NAME": "brightness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall brightness." },
        { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall saturation." },
        { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0, "DESCRIPTION": "Adjusts overall contrast." },
        { "NAME": "gamma_correction", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Gamma correction for final color." },
        { "NAME": "vignette_intensity", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Intensity of the vignette effect." }
    ]
}
*/

#define R(p,a,r)mix(a*dot(p,a),p,cos(r))+sin(r)*cross(p,a) // Re-added R macro
#define PI 3.141592
#define TAU 	(PI * 2.0)
#define HEX_COS_VAL (0.86602540378443 * 0.5) // Original HEX_COS
#define HEX_TAN_VAL (0.57735026918962 * 0.5) // Original HEX_TAN
#define SIN_NORM(x) (sin(x)*.5+.5) // Normalized sine 0-1

// Original hue function from the shader
vec3 hue(float v) { return ( .6 + .6 * cos( 6.3*(v) + vec3(0,23,21) ) ); }

// Custom psychedelic color palettes
vec3 getColorPalette(float t, float type) {
    if (type < 0.5) return hue(t); // Palette 0: Original shader's hue
    if (type < 1.5) return vec3(sin(t * 5.0), sin(t * 7.0 + 1.0), sin(t * 9.0 + 2.0)) * 0.5 + 0.5; // Palette 1: Rapid Sine Waves
    if (type < 2.5) return vec3(cos(t * 4.0 + 2.0), cos(t * 2.0 + 1.0), sin(t * 6.0)) * 0.5 + 0.5; // Palette 2: Muted Cosine Blends
    if (type < 3.5) return vec3(sin(t * 2.0), sin(t * 4.0), cos(t * 8.0)) * 0.5 + 0.5; // Palette 3: Fast RGB Pulse
    if (type < 4.5) return vec3(fract(t * 3.0), fract(t * 5.0), fract(t * 7.0)); // Palette 4: Hard Edge Fractal Colors
    if (type < 5.5) return vec3(sin(t * 1.5), cos(t * 3.0), sin(t * 4.5 + cos(t * 2.0))) * 0.5 + 0.5; // Palette 5: Organic Swirl
    if (type < 6.5) return mix(hue(t * 0.7), hue(t * 1.3 + 0.5), 0.5); // Palette 6: Dual Hue Blend
    // Palette 7: Chakra-like static colors (can be animated if desired)
    vec3 cols[7] = vec3[](vec3(0.608,0.020,1.000), vec3(0.169,0.059,1.000), vec3(0.000,0.800,1.000),
                          vec3(0.035,1.000,0.020), vec3(0.984,1.000,0.161), vec3(1.000,0.463,0.020),
                          vec3(1.000,0.000,0.000));
    int index = int(mod(t * 7.0, 7.0)); // Cycle through chakra colors based on t
    return cols[index];
}


// Function to adjust brightness, saturation, contrast
vec3 adjustColor(vec3 color, float br, float sat, float con) {
    color = mix(vec3(0.5), color, con); // Adjust contrast around 0.5 (mid-gray)
    vec3 gray = vec3(dot(color, vec3(0.299, 0.587, 0.114))); // Calculate grayscale luminance
    color = mix(gray, color, sat); // Adjust saturation by mixing with grayscale
    return color * br; // Adjust brightness
}

// Simple hash functions for noise for glitch/shake
float hash11(float p) { p = fract(p * .1031); p *= p + 33.33; p *= p + p; return fract(p); }
float hash22(vec2 p) { return fract(sin(dot(p, vec2(41.45, 12.04))) * 9876.5432); }

// Global variables from original shader, mapped to ISF flow where possible or made local
float tt_global; // Corresponds to `tt`
float g_mat_global; // Corresponds to `g_mat`
float bd_global; // Corresponds to `bd` (body distance)
vec3 ro_global; // Corresponds to `ro` (ray origin)

// 2D rotation matrix
mat2 rot2(float a) { return mat2(cos(a), sin(a), -sin(a), cos(a)); }

// Distance to Hexagon
float dHex(vec2 p) {
    p = abs(p);
    float c = dot(p, normalize(vec2(1, 1.73)));
    return max(c, p.x);
}

// Distance to Triangle (from The Book of Shaders)
float dTri(vec2 p) {
    float a = atan(p.x,p.y)+PI;
    float r = TAU/float(3);
    float d = cos(floor(.5+a/r)*r-a)*length(p);
    return d;
}

// Hexagonal ring pattern
float hexRing(vec2 p, float r, float s, float blur) {
    float d = dHex(p) - r ;
    float c = 1.-smoothstep(0., s, abs(d)-blur);
    return c;
}

// Triangular ring pattern
float triRing(vec2 p, float r, float s, float blur) {
    float d = dTri(p) - r ;
    float c = 1.-smoothstep(0., s, abs(d)-blur);
    return c;
}

// Circular ring pattern
float ring(vec2 p, float r, float s, float blur) {
    float d = length(p) - r ;
    float c = 1.-smoothstep(0., s, abs(d)-blur);
    return c;
}

// Polar modulo operation for 2D patterns
vec2 pmod(vec2 pos, float num, out float id) {
    float angle = atan(pos.x, pos.y) + PI / num;
    float split = TAU / num;
    id = floor(angle / split);
    angle = id * split;
    return rot2(angle) * pos;
}

// Curved interpolation (by Nusan)
float curve(float t, float d) {
    t/=d;
    return mix(floor(t), floor(t)+1., pow(smoothstep(0.,1.,fract(t)), 20.));
}

// 3D repetition helper
vec3 repeat_3D(inout vec3 p, vec3 size) {
    vec3 c = floor((p + size*0.5)/size);
    p = mod(p + size*0.5, size) - size*0.5;
    return c;
}

// 1D repetition helper
float repeat_1D(inout float p, float size) {
    float c = floor((p + size*0.5)/size);
    p = mod(p + size*0.5, size) - size*0.5;
    return c;
}

// SDF for a pill (capsule) shape (from iq)
float sdPill( vec3 p, vec3 a, vec3 b, float r ) {
    vec3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

// Distance to lines for the background star pattern
void mapStars(vec2 uv, out vec3 near, out vec3 neighbor) {
    vec2 point;
    near = vec3(1e+4);

    for(float y=-1.0; y<=1.0; y+=2.0) {
        point = vec2(0.0, HEX_COS_VAL + y * HEX_TAN_VAL * 0.25);
        float dist = distance(uv, point);
        near = near.z < dist ? near : vec3(point, dist);
    }
    
    for(float x=-1.0; x<=1.0; x+=2.0) {
        for(float y=-1.0; y<=1.0; y+=2.0) {
            for(float both=-1.0; both<=1.0; both+=2.0) {
                point = vec2(x * 0.125, HEX_COS_VAL + y * HEX_COS_VAL * 0.5);
                point.x += both * 0.5     * 0.125 * -x;
                point.y += both * HEX_TAN_VAL * 0.125 * -y;
                float dist = distance(uv, point);
                near = near.z < dist ? near : vec3(point, dist);
            }
        }
    }
    
    neighbor = vec3(1e+4);
    
    for(float y=-1.0; y<=1.0; y+=2.0) {
        point = vec2(0.0, HEX_COS_VAL + y * HEX_TAN_VAL * 0.25);
        if(near.xy != point) { // Check if this is not the nearest point
            vec2 center = (point + near.xy) * 0.5;
            float dist = dot(uv - center, normalize(near.xy - point));
            neighbor = neighbor.z < dist ? neighbor : vec3(point, dist);
        }
    }
    
    for(float x=-1.0; x<=1.0; x+=2.0) {
        for(float y=-1.0; y<=1.0; y+=2.0) {
            for(float both=-1.0; both<=1.0; both+=2.0) {
                point = vec2(x * 0.125, HEX_COS_VAL + y * HEX_COS_VAL * 0.5);
                point.x += both * 0.5     * 0.125 * -x;
                point.y += both * HEX_TAN_VAL * 0.125 * -y;
                
                if(near.xy != point) { // Check if this is not the nearest point
                    vec2 center = (point + near.xy) * 0.5;
                    float dist = dot(uv - center, normalize(near.xy - point));
                    neighbor = neighbor.z < dist ? neighbor : vec3(point, dist);
                }
            }
        }
    }
}

// Log-polar transform
vec2 toLogPolar(vec2 p) {
    // Handle p.x = 0 to avoid atan(inf) issues
    if (abs(p.x) < 0.00001) {
        return vec2(log(length(p)), p.y > 0.0 ? PI/2.0 : -PI/2.0);
    }
    return vec2(log(length(p)), atan(p.y,p.x));
}

// Star background pattern function
float starPattern(vec2 uv, float _time, float width_mult, float density_mult, float alpha_blend) {
    vec2 uvb = uv;
    // Use the ISF input 'line_width'
    float width = (line_width * width_mult) + mix(0.03, 0., pow(dot(uv, uv), .3)); // Tunable line width multiplier
    
    // log polar transform
    uv = toLogPolar(uv * density_mult * 0.01)*2.5; // Tunable density
    uv.x += -0.2 * _time;
    
    uv = vec2(mod(uv.x, 1.0) - 0.5,
              mod(uv.y, HEX_COS_VAL * 2.0) - HEX_COS_VAL);
    
    // rot tiling
    float id;
    
    // change the patterns switching angles on a radial/time base
    float reps = 5.;
    float t_star = 0.07*(_time+6.);
    float modid = (mod(floor(0.1*length(uv)-t_star), reps)+3.)*2.;
    float modt = pow(smoothstep(0.0, 0.3, abs(fract(0.1*length(uvb)-t_star)-0.5)), 500.);
    
    float alpha = mix(6., 18., modt);
    alpha = mix(alpha, 18., alpha_blend); // Tunable blend for fixed vs dynamic alpha
    uv = pmod(uv, alpha, id);
        
    // scene
    vec3 near_star, neighbor_star;
    mapStars(uv, near_star, neighbor_star);
 
    // draw
    float line = (1.0 - smoothstep(0.0, width, neighbor_star.z));
    
    return line;
}

// Kaleidoscopic fractal for the interior (KIFS-like)
vec3 kalei(vec3 p, float _time, float morph_speed, float z_factor, float y_factor) {
    p.x = abs(p.x) - 2.5;
    
    vec3 q = p;
    q.y -= 0.5;
    q.y += 0.4*sin(_time * morph_speed); // Tunable morph speed
    p.y += 0.3*sin(p.z*3.0 + 0.5*_time * morph_speed); // Tunable morph speed
    float at = length(q) - 0.01;

    for(float i=0.; i < 6.; i++) {      
        p.x = abs(p.x) - 1.5;
    
        p.xz *= rot2(1.-exp(-p.z * z_factor * i)+0.2*_time * morph_speed + 0.1*at); // Tunable z_factor, morph speed
        p.xy *= rot2(sin(2.*i)+0.2*_time * morph_speed); // Tunable morph speed
        
        p.y += 1.-exp(-p.z * y_factor * i); // Tunable y_factor
    }
    p.x = abs(p.x) + 2.5;
        
    return p;
}

// Smooth Intersection (from iq)
float opSmoothIntersection( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
    return mix( d2, d1, h ) + k*h*(1.0-h); 
}

// Smooth Union (from iq)
float opSmoothUnion( float d1, float d2, float k ) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return mix( d2, d1, h ) - k*h*(1.0-h); 
}
    
// Main distance field for the primary fractal
float map(vec3 p, float _time, float mesh_density_mult_input, float layer_dist, float line_w, 
          float body_rad, float body_morph, float head_rad, float blend_factor) {

    vec3 bp = p; // Store original position for body SDF

    // Glowing interior / KIFS fractal of log polar line mesh
    p.yz *= rot2(-PI*0.25);
    p = kalei(p, _time, kalei_morph_speed, kalei_z_factor, kalei_y_factor); // Pass tunable params to kalei
    
    // map to log-spherical coordinates
    float r = length(p);
    // Avoid log(0) and atan(0/0) issues
    if (r < 0.00001) r = 0.00001; 
    float theta = acos(clamp(p.z / r, -1.0, 1.0)); // Clamp to avoid NaN from float precision
    float phi = atan(p.y, p.x);

    p = vec3(log(r), theta, phi);
    
    // some heuristic computation to compensate the log scaling
    float shrink = 1.0/max(0.0001, abs(p.y-PI)) + 1.0/max(0.0001, abs(p.y)) - 1.0/PI; // Avoid division by zero
    
    // Use the ISF input 'mesh_density_mult'
    float scale = floor(90.0 * mesh_density_mult_input)/PI; // Original MESH_DENSITY was 90.
    p *= scale;
    
    p.x -= _time; // Global time movement
    p.y -= 0.7; // Vertical offset
    
    vec3 id = repeat_3D(p, vec3(layer_dist, 0.5, 0.5)); // Tunable layer distance

    p.yz *= rot2(0.25*PI);
    p.x *= shrink;
        
    g_mat_global = bp.y*0.6+id.x+abs(bp.x*0.2); // Global material ID for coloring
    
    float w = line_w; // Tunable line width (from ISF input)
    float d = length(p.xz) - w;
    d = min(d, length(p.xy) - w);
    d *= r/(scale*max(0.0001, shrink)); // Avoid division by zero
    
    // Body (SDF for a character-like shape)
    bp.z *= 0.5;
    bp.z -= 0.75;
    
    vec3 bp1 = bp;
    bp1.x *= mix(1.0, 3.0, smoothstep(1.15, 1.3, bp1.y)); // Horizontal morph
    r = mix(1.2, 0.8, smoothstep(1.1, -0.0, abs(bp1.y)-0.2)); // Vertical morph influence
    bd_global = sdPill(bp1, vec3(0, -0.14, 0), vec3(0, 0.7 + body_morph, 0), r * body_rad); // Tunable body_morph and body_rad
    
    // Head
    bp1 = bp;
    bp1.x *= 1.0;
    bd_global = opSmoothUnion(bd_global, length(bp1-vec3(0, 2.61, 0)) - head_rad, 1.5); // Tunable head_rad
    
    // Legs
    bp1 = bp;
    bp1.z *= 0.6;
    bp1.z -= 0.6;
    bp1.x = -abs(bp1.x);
    
    vec3 p1 = vec3(0, -1.3, 0);
    vec3 p2 = vec3(-1.4, -0.8, -1.3);
    r = mix(0.5, 0.31, dot(bp1, normalize(p2-p1)));
    bd_global = opSmoothUnion(bd_global, sdPill(bp1, p1, p2, r), 0.5);
    
    // Arms
    bp1 = bp;
    bp1.z *= 0.5;
    bp1.z -= 0.1;
    bp1.x = -abs(bp1.x);
    p1 = vec3(-1.15, 1.2, -0.1);
    p2 = vec3(-1.4, -0.85, -0.2);
    r = 0.13;
    bd_global = opSmoothUnion(bd_global, sdPill(bp1, p1, p2, r), 0.4);
    
    // Intersect body volume with the fractal
    d = opSmoothIntersection(d, bd_global - 0.4, blend_factor); // Tunable blend factor
    
    return d*0.5;
}

// 2D radial modulo
vec2 moda(vec2 p, float repetitions) {
    float angle = TAU/repetitions;
    float a = atan(p.y, p.x) + angle/2.;
    a = mod(a,angle) - angle/2.;
    return vec2(cos(a), sin(a))*length(p);
}

// Rings pattern (outer background element)
vec3 rings(vec2 uv, float _time, float t_tri_size, float t_hex_size, float t_circle_size,
           float t_line_width_mult, float t_blur, float t_glow_intensity, float t_anim_rate,
           vec3 base_ring_color, float ring_mix_hue, float ring_pulsing_mix) {
    float rings_val = 0.;
    vec2 uvr = uv;
    uvr *= 1.2;

    float current_line_width = line_width * t_line_width_mult; // Use ISF input line_width

    // The KALEI preprocessor directive from Shadertoy is removed.
    // If you want to enable this, you would uncomment/add its logic here
    // for(float i=0.; i< 3.; i++) {
    //     uvr = abs(uvr) - t_tri_size;
    //     uvr *= rot2(cos(uvr.x+_time));
    // }

    rings_val += hexRing(uvr, t_hex_size, current_line_width, t_blur);    
    rings_val += t_glow_intensity*hexRing(uvr, t_hex_size, current_line_width*5., t_blur); //glow

    vec2 uv0 = uvr;
    uv0 *= rot2(-PI*t_anim_rate*curve(_time, 2.)); // Tunable anim rate
    rings_val += hexRing(uv0, t_hex_size*0.5, current_line_width, t_blur);
    rings_val += t_glow_intensity*hexRing(uv0, t_hex_size*0.5, current_line_width*5., t_blur);

    vec2 uv1 = uvr;
    uv1 *= rot2(PI*t_anim_rate*(curve(_time, 2.))); // Tunable anim rate
    rings_val += triRing(uv1, 0.286, current_line_width, t_blur);
    rings_val += t_glow_intensity*triRing(uv1, 0.286, current_line_width*5., t_blur); // glow

    vec2 uv2 = uvr;
    uv2 *= rot2(PI-t_anim_rate*PI*((curve(_time, 2.)))); // Tunable anim rate
    rings_val += triRing(uv2, t_tri_size, current_line_width, t_blur);
    rings_val += t_glow_intensity*triRing(uv2, t_tri_size, current_line_width*5., t_blur); // glow

    vec2 uv3 = uvr;
    uv3 *= rot2(PI*0.5);
    uv3 *= rot2(PI*t_anim_rate*curve(_time, 2.)); // Tunable anim rate

    uv3 = moda(uv3, 6.);
    uv3.x -= mix(t_tri_size, 0.576, abs(mod(curve(_time, 2.), 2.)-1.));
    rings_val += ring(uv3, t_circle_size, current_line_width, t_blur);
    rings_val += t_glow_intensity*ring(uv3, t_circle_size, current_line_width*5., t_blur);
    
    vec3 col = rings_color_mix * mix(base_ring_color, hue(-_time + length(uv)*2.0 + 0.5*PI), ring_mix_hue) * rings_val * mix(0.0, 0.5, SIN_NORM(_time * PI)); // Tunable mix, intensity, pulsing
    return col;
}

// Chakras pattern (layered background element)
vec3 chakras(vec2 uv, float _time, float intensity) {
    vec3 cols[7] = vec3[](vec3(0.608,0.020,1.000), vec3(0.169,0.059,1.000), vec3(0.000,0.800,1.000),
                          vec3(0.035,1.000,0.020), vec3(0.984,1.000,0.161), vec3(1.000,0.463,0.020),
                          vec3(1.000,0.000,0.000));
    float offs[7] = float[](.48, .355, .24, .12, 0., -.1, -.19);
    vec3 col = vec3(0);
    
    for(int i = 0; i < 7; i++) {
        vec2 coords = uv - vec2(0, offs[i]);
        float anim = 0.1+0.9*SIN_NORM(-4.0*_time+2.0*PI*float(i)/7.0);
        col += cols[i]*mix(1.0, 0.0, smoothstep(0.0, 0.28, pow(length(coords), 0.2)-0.2))*anim;
    }
    
    return col * intensity; // Tunable intensity
}


void main() {
    vec4 fragColor = vec4(0);
    vec2 fragCoord = gl_FragCoord.xy;

    float current_time = TIME * speed; // Global time with speed control
    tt_global = 0.3 * current_time; // Map tt to a global for original shader usage

    // Standard ISF resolution handling
    vec2 uv_normalized = (fragCoord - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    uv_normalized /= zoom; // Apply zoom

    // Camera ray direction
    vec3 rd = normalize(vec3(uv_normalized, 0.7)); // Original constant Z value
    
    // Apply camera rotation based on xy_control_vec
    vec2 xy_control_vec = vec2(xy_control_x, xy_control_y);
    rd = R(rd, vec3(0.577), xy_control_vec.x * TAU); // Full 2PI rotation for X
    rd = R(rd, vec3(0.577), xy_control_vec.y * PI); // Half PI rotation for Y (to prevent flip)

    // Apply camera shake directly to initial ray direction
    if (shake_strength > 0.001) {
        vec2 shake_uv_norm = (fragCoord / RENDERSIZE.xy) - 0.5;
        shake_uv_norm.x *= RENDERSIZE.x / RENDERSIZE.y; 
        
        vec2 shake_offset = vec2(
            sin(current_time * shake_frequency + hash11(1.0)) * 0.1,
            cos(current_time * shake_frequency * 1.1 + hash11(2.0)) * 0.1
        ) * shake_strength;
        
        rd = R(rd, normalize(vec3(shake_offset.x, shake_offset.y, 0.0)), length(shake_offset) * 0.1);
    }

    // Apply glitch effect directly to initial ray direction
    if (glitch_strength > 0.001) {
        vec2 glitch_uv_norm = fragCoord / RENDERSIZE.xy;
        float offset_x_noise = (hash22(glitch_uv_norm * 10.0 + current_time * glitch_frequency) - 0.5) * 2.0;
        float scanline_strength = sin(glitch_uv_norm.y * 150.0 + current_time * 20.0) * 0.5 + 0.5;
        
        rd.x += offset_x_noise * scanline_strength * glitch_strength * 0.05;
        rd = normalize(rd);
    }

    ro_global = vec3(0, 0.0, -4.0); // Fixed ray origin as per original

    vec3 p = ro_global; // Ray position
    vec3 final_color_accum = vec3(0);
    
    float current_ray_dist = 0.1; // Initial distance for raymarcher, `t` in original
    
    float mat_val_loop = 0.; // Material value accumulated per loop, local copy of g_mat_global
    
    vec3 background_color = vec3(0.016,0.086,0.125); // Original background color

    for(float i=0.0; i<raymarch_steps; i++) { // Tunable raymarch steps
        
        float d_val = map(p, current_time, mesh_density_mult, layer_distance, line_width, 
                          body_pill_radius, body_pill_height_morph, head_radius, smooth_blend_fractal_body);
        mat_val_loop = g_mat_global; // Update material from map function call
        
        // Break if ray distance exceeds max or hit detected
        if(current_ray_dist > max_ray_distance) break; // Tunable max ray distance
        
        current_ray_dist += max(0.01, abs(d_val)); // Increment ray distance
        p += rd*d_val; // Move ray position
        
        // Shading / color accumulation (direct translation of original logic)
        if(d_val < min_step_distance) { // Tunable min step distance
            // Base fractal color
            final_color_accum += getColorPalette(mat_val_loop * 0.4, color_palette_type) * fractal_color_intensity / exp(current_ray_dist * 0.6);
        }
        
        // Body glow/outline effect
        if(abs(bd_global - 0.04) < 0.0005 && bd_global < 0.04) {
            final_color_accum += 1.1 - exp(-bd_global * bd_global * 50.0);
        }
    }   
    
    // Background rendering if fractal not strongly hit
    if(dot(final_color_accum, final_color_accum) < 0.001) {
        // Solid background color
        final_color_accum += background_color * mix(background_color_mix, 1.1, (1.0 - pow(dot(uv_normalized, uv_normalized), 0.5))); // Tunable mix
        
        // Star pattern
        float stars = starPattern(uv_normalized, current_time, star_pattern_width_mult, star_pattern_density, star_pattern_mod_alpha_blend);
        final_color_accum += stars * star_color_mix * (0.1 + star_hue_intensity * getColorPalette(-current_time + length(uv_normalized) * 2.0, color_palette_type) * (0.1 + star_pulse_intensity * SIN_NORM(length(uv_normalized)*1.5+current_time))); // Tunable
        
        // Rings pattern
        vec3 ring_base_color = vec3(0.761,0.851,1.000); // Original constant color for rings
        final_color_accum += rings_color_mix * rings(uv_normalized, current_time, ring_tri_size, ring_hex_size, ring_circle_size,
                                                    ring_line_width_mult, ring_blur, ring_glow_intensity, ring_animation_rate,
                                                    ring_base_color, rings_pulsing_mix, rings_pulsing_mix); // Tunable
    }
    
    // Chakra overlay (added on top regardless of hit)
    final_color_accum = 0.8 * final_color_accum + chakras_intensity * chakras(uv_normalized, current_time, chakras_intensity); // Tunable intensity
    
    // Final vignette (fixed aesthetic, added as tunable for intensity)
    final_color_accum *= mix(0.1, 1.0, (1.5 - pow(dot(uv_normalized, uv_normalized), 0.2))); 
    final_color_accum *= (1.0 - vignette_intensity * pow(length(uv_normalized), 2.0)); // A more common vignette style

    // Gamma correction (tunable)
    final_color_accum = pow(final_color_accum, vec3(gamma_correction));

    // Brightness, Saturation, Contrast
    final_color_accum = adjustColor(final_color_accum, brightness, saturation, contrast);

    // Final output, with alpha channel set by ray distance
    fragColor = vec4(clamp(final_color_accum, 0.0, 1.0), 1.0 - current_ray_dist * 0.3);
    gl_FragColor = fragColor;
}