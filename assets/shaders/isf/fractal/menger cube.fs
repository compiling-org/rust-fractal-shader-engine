/*
{
  "DESCRIPTION": "Fractal Tunnel with Advanced Color Pulse & Shake Controls (Based on User's Reference)",
  "CATEGORIES": [ "Generator", "Fractal", "Tunnel", "Psychedelic" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "inputTex1", "TYPE": "image" },
    { "NAME": "inputTex2", "TYPE": "image" },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 },
    { "NAME": "morph", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "shakeAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "shakeSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "shakePhase", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "glitchAmount", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "colorPulse", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "colorPulseFreq", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0 },
    { "NAME": "colorPulseShape", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 3.9 },
    { "NAME": "flashIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "flashColorR", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "flashColorG", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "flashColorB", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "paletteType", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 4.9 },
    { "NAME": "camX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -2.0, "MAX": 2.0 },
    { "NAME": "camZ", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 4.0 },
    { "NAME": "fractal1", "TYPE": "float", "DEFAULT": 2.0, "MIN": 0.5, "MAX": 4.0 },
    { "NAME": "fractal2", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 6.0 },
    { "NAME": "fractal3", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 9.0 },
    { "NAME": "brightness", "TYPE": "float", "DEFAULT": 10.0, "MIN": 0.0, "MAX": 100.0 },
    { "NAME": "saturation", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 2.0 },
    { "NAME": "contrast", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 3.0 },
    { "NAME": "vignetteStrength", "TYPE": "float", "DEFAULT": 0.1, "MIN": 0.0, "MAX": 0.5 },
    { "NAME": "camera_control_type", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0 },
    { "NAME": "camera_yaw", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "camera_pitch", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 }
  ]
}
*/

#define PI 3.141592653589793
#define TAU (PI * 2.0)
#define inf 9e9

// Global constants for raymarching (hardcoded as they are not in the provided JSON inputs)
#define MAX_DIST 100.0 // Max distance ray can travel
#define SURF_PRECISION 0.001 // Precision for surface hit detection
#define MAX_STEPS 200 // Max raymarching steps

// Global Time based on 'speed' input (ISF provides TIME automatically)
float t = TIME * speed;

// Function Prototypes (forward declarations for functions called before their full definition)
vec3 getLightPosition();
float map(vec3 p); // Prototype for map, as it's used in AO and normal

// Dynamic Light Source position (parameters hardcoded as they are not in the provided JSON inputs)
vec3 getLightPosition() {
    const float light_orbit_speed_val = 1.0; 
    const float light_orbit_radius_val = 3.0; 
    const float light_scroll_speed_val = 0.5;

    return vec3(cos(t * light_orbit_speed_val) - 1.0, sin(t * light_orbit_speed_val), cos(t * light_orbit_speed_val * 0.5)) * light_orbit_radius_val - vec3(t * light_scroll_speed_val, 0.0, 0.0);
}

// --- Color Palette Functions ---
vec3 applyPalette(float value, float time_multiplier) {
    vec3 color = vec3(0.0);
    float p = floor(paletteType + 0.5);
    float t_scaled = time_multiplier * 0.5; // Use time_multiplier for palette animation

    // Ensure value cycles between 0 and 1 for consistent palette behavior
    value = fract(value); 

    if (p == 0.0) { // Psychedelic Rainbow
        color = 0.5 + 0.5 * cos(6.28318 * (vec3(value, value + 0.166, value + 0.333) * 2.0 + t_scaled));
    } else if (p == 1.0) { // Fiery Sunset
        color = mix(vec3(0.8, 0.0, 0.0), vec3(1.0, 0.6, 0.0), value);
        color = mix(color, vec3(1.0, 0.9, 0.5), smoothstep(0.7, 1.0, value));
        color = mix(color, vec3(0.2, 0.0, 0.2), 0.5 - abs(value - 0.5)); // Adds a deeper tone in the middle
    } else if (p == 2.0) { // Deep Ocean Blues
        color = mix(vec3(0.0, 0.1, 0.3), vec3(0.0, 0.6, 1.0), value);
        color = mix(color, vec3(0.5, 1.0, 1.0), smoothstep(0.7, 1.0, value));
    } else if (p == 3.0) { // Electric Greens & Purples
        color = mix(vec3(0.1, 0.8, 0.1), vec3(0.8, 0.2, 0.8), value);
        color = mix(color, vec3(0.3, 1.0, 0.3), smoothstep(0.8, 1.0, value));
        color = mix(color, vec3(0.6, 0.1, 0.6), 0.5 - abs(value - 0.5));
    } else { // Golden Tones
        color = mix(vec3(0.5, 0.4, 0.1), vec3(1.0, 0.8, 0.2), value);
        color = mix(color, vec3(1.0, 1.0, 0.8), smoothstep(0.7, 1.0, value));
    }
    return clamp(color, 0.0, 1.0);
}

float colorPulseValue(float time_val) {
    float shape = floor(colorPulseShape + 0.5);
    float v = sin(time_val * colorPulseFreq);
    // This value is a multiplier for the *dynamic light elements* (like the tex3D and light).
    // It controls the amplitude and shape of the pulse.
    if (shape == 0.0) return 1.0 + colorPulse * v * 0.3;
    if (shape == 1.0) return 1.0 + colorPulse * abs(v) * 0.4;
    if (shape == 2.0) return 1.0 + colorPulse * smoothstep(-1., 1., v) * 0.5;
    if (shape == 3.0) return 1.0 + colorPulse * step(0., v) * 0.6;
    return 1.0;
}

// P(z) from reference: defines the path of the tunnel
vec3 P(float z) {
    return vec3(cos(z * 0.1) * 4. + tanh(cos(z * 0.1) * 1.8) * 4.,
                tanh(cos(z * 0.15) * 0.4) * 8.,
                z);
}

// 2D Rotation matrix
mat2 rot(float a) {
    float c = cos(a), s = sin(a);
    return mat2(c, -s, s, c);
}

// Custom length2 function from reference
float length2(vec2 p){
    float k = (sin(t * 0.1) * 8. + 4.) * morph; // Morph affects this
    p = pow(abs(p), vec2(k));
    return pow(p.x + p.y, 1. / k);
}

// tex3D_grayscale from reference: samples texture and returns grayscale value
float tex3D_grayscale(sampler2D tex, vec3 p, vec3 n) {
    vec3 nn = max((abs(n) - 0.2) * 7., 0.001);
    nn /= (nn.x + nn.y + nn.z);
    vec2 uv1 = fract(vec2(p.y, p.z));
    vec2 uv2 = fract(vec2(p.z, p.x));
    vec2 uv3 = fract(vec2(p.x, p.y));
    
    // Convert sampled texture to grayscale using luminance, then blend with normals
    float g1 = dot(texture2D(tex, uv1).rgb, vec3(0.2126, 0.7152, 0.0722));
    float g2 = dot(texture2D(tex, uv2).rgb, vec3(0.2126, 0.7152, 0.0722));
    float g3 = dot(texture2D(tex, uv3).rgb, vec3(0.2126, 0.7152, 0.0722));

    return g1 * nn.x + g2 * nn.y + g3 * nn.z;
}

// Fractal function from reference (Menger-like)
float fractal(vec3 q){
    vec3 p = q;
    float d = inf, s = 2.; // s is the current scale
    #define MENGER(scale_val, minmax_func, hole_val) \
        s /= scale_val; \
        p = abs(fract(p/s)*s - s*.5); \
        d = minmax_func(d, min(max(p.x, p.y), min(max(p.y, p.z), max(p.x, p.z))) - s / hole_val);
    
    // Applying the Menger-like operations with fractal parameters
    MENGER(fractal1, min, 3.0); // Union operation
    MENGER(fractal2, max, 3.0); // Intersection operation
    MENGER(fractal3, max, 3.0); // Another intersection operation
    #undef MENGER // Undefine macro to avoid pollution

    return d; // This 'd' is the distance to the Menger-like fractal
}

// Tunnel function from reference
float tunnel(vec3 p) {
    // This defines the outer boundary of the tunnel, which the fractal carves into.
    return (sin(p.z * .6) + 2.) -
           min(length(p.xy - P(p.z).x + 4.),
           min(length(p.x - p.y - 10.),
           min(length2(p.xy - P(p.z).xy),
               length2(p.xy - vec2(P(p.z).y)))));
}

// Combined map function from reference: fractal carves into tunnel
float map(vec3 p) {
    return max(tunnel(p), fractal(p));
}

// Ambient Occlusion function from reference (ao_intensity hardcoded)
float AO(vec3 pos, vec3 nor) {
    const float ao_intensity_val = 1.0; // Hardcoded default for ao_intensity
    float sca = 2.0, occ = 0.0;
    for (int i = 0; i < 5; i++) {
        float hr = 0.01 + float(i) * 0.5 / 4.0;
        float dd = map(nor * hr + pos); // Use 'map' here
        occ += (hr - dd) * sca;
        sca *= 0.7;
    }
    return clamp(1.0 - occ, 0.0, 1.0);
}

// Post-processing function applying BCS and color pulse
vec3 post(vec3 c, float pulse_multiplier) {
    c = (c - 0.5) * contrast + 0.5;
    float gray = dot(c, vec3(0.2126, 0.7152, 0.0722));
    c = mix(vec3(gray), c, saturation);
    c *= brightness;
    
    // Apply pulse_multiplier here to globally scale the output intensity/brightness with the pulse.
    c *= pulse_multiplier; 
    return c;
}

void main() {
    // Normalized fragment coordinates, centered and scaled by aspect ratio
    vec2 u = isf_FragNormCoord * RENDERSIZE - RENDERSIZE * 0.5;
    u /= RENDERSIZE.y;

    // Camera Position (ro) and LookAt target (lookAt)
    vec3 ro; // Ray Origin
    vec3 Z_cam; // Camera's forward (Z) axis
    vec3 X_cam; // Camera's right (X) axis
    vec3 Y_cam; // Camera's up (Y) axis

    // Hardcoded camera parameters not in JSON
    const float camera_offset_forward_factor_val = 0.0; // Default to 0 for automatic mode
    const float camera_fov_val = 1.0; // Default FOV

    if (camera_control_type > 0.5) { // Manual Control
        // Manual yaw and pitch angles from inputs
        float yaw_angle = (camera_yaw - 0.5) * TAU; // Scale yaw to 0-2PI
        float pitch_angle = (camera_pitch - 0.5) * PI; // Scale pitch to 0-PI

        // Calculate camera's forward vector based on manual angles
        Z_cam = vec3(cos(yaw_angle) * cos(pitch_angle), sin(yaw_angle) * cos(pitch_angle), sin(pitch_angle));
        
        // Calculate camera's right and up vectors
        X_cam = normalize(cross(Z_cam, vec3(0.0, 1.0, 0.0))); // Assuming world Y is up
        Y_cam = normalize(cross(X_cam, Z_cam));

        // Camera position for manual control: fixed X/Y, Z scrolls, plus an offset along forward direction
        ro = vec3(camX, camY, t * camZ) + Z_cam * camera_offset_forward_factor_val; // Using hardcoded offset

    } else { // Automatic Control
        // Automatic camera path for tunnel effect: moves along Z-axis
        ro = vec3(camX, camY, t * camZ); 
        
        // Automatic lookAt point: slightly ahead of camera, with some wobbly motion
        vec3 lookAt = vec3(camX + sin(t * 0.1) * 0.5, camY + cos(t * 0.08) * 0.5, ro.z + 1.0); 
        
        // Calculate camera basis vectors from automatic lookAt
        Z_cam = normalize(lookAt - ro);
        X_cam = normalize(cross(Z_cam, vec3(0.0, 1.0, 0.0)));
        Y_cam = normalize(cross(X_cam, Z_cam));
    }

    // Apply camera shake to screen_uv
    if (shakeAmount > 0.0) { 
        float shake_phase_val = sin(t * shakeSpeed + shakePhase * 6.28);
        // Apply shake as an offset to the screen UVs, scaled by shakeAmount
        u += vec2(sin(t * shakeSpeed * 250.), cos(t * shakeSpeed * 570.)) * 7.0 * shakeAmount * shake_phase_val / RENDERSIZE.y;
    }

    // Calculate the final ray direction in world space.
    // This correctly uses the camera's Right (X_cam), Up (Y_cam), and Forward (Z_cam) vectors.
    // camera_fov_val is used as a multiplier for the screen UVs to control FOV.
    vec3 ray_direction = normalize(X_cam * u.x * camera_fov_val + Y_cam * u.y * camera_fov_val + Z_cam * 1.0);

    // Raymarching loop
    float d = 0.; // Total distance traveled by ray
    float s = 0.0; // Current step size
    vec3 p_hit; // Point where ray hits

    // Loop for raymarching steps, limited by MAX_STEPS, SURF_PRECISION, and MAX_DIST.
    for (int iter = 0; iter < MAX_STEPS; iter++) { 
        p_hit = ro + ray_direction * d;
        s = map(p_hit); // Get distance to the fractal from current point
        d += s * 0.35; // Advance ray by the calculated distance (0.35 from reference)

        if (s < SURF_PRECISION || d > MAX_DIST) { 
            break; // Stop if hit surface or went too far
        }
    }
    
    vec3 final_color = vec3(0.0); // Initialize final color to black (background)

    // Check if ray hit the fractal surface
    if (d < MAX_DIST) { 
        // Calculate surface normal at the hit point using central differencing
        vec3 r_normal = normalize(vec3(
            map(p_hit - vec3(SURF_PRECISION, 0, 0)) - map(p_hit + vec3(SURF_PRECISION, 0, 0)), 
            map(p_hit - vec3(0, SURF_PRECISION, 0)) - map(p_hit + vec3(0, SURF_PRECISION, 0)), 
            map(p_hit - vec3(0, 0, SURF_PRECISION)) - map(p_hit + vec3(0, 0, SURF_PRECISION))  
        ));

        // Get grayscale base texture value from inputTex1 or inputTex2
        // The original reference uses mod(p.z, 10.0) > 5.0 to switch textures
        float base_texture_val = mod(p_hit.z, 10.0) > 5.0 ?
            tex3D_grayscale(inputTex1, p_hit * 2.5, r_normal) :
            tex3D_grayscale(inputTex2, p_hit * 1.5, r_normal) * 2.0;

        // Apply palette to the grayscale base texture value to get desired vibrant color
        final_color = applyPalette(fract(p_hit.z * 0.1), t); // Use fract(p_hit.z * 0.1) for palette input

        // Apply basic lighting (dot product with light direction)
        // Hardcoded lighting parameters (not in JSON inputs)
        const float diffuse_strength_val = 1.0; 
        const float specular_strength_val = 1.0; 
        const float specular_power_val = 32.0; 
        const float light_attenuation_factor_val = 5.0; 

        // Ambient Occlusion
        float L_ao = AO(p_hit, r_normal); // AO function from reference
        
        vec3 light_pos = getLightPosition();
        vec3 D_light = normalize(light_pos - p_hit);

        float L_diffuse = max(dot(r_normal, D_light), 0.1) * diffuse_strength_val; 
        float L_attenuation = exp2(1.0 - length(light_pos - p_hit) / light_attenuation_factor_val); 
        
        final_color *= L_diffuse * L_attenuation * L_ao;

        // Specular highlight
        vec3 reflected_light = reflect(D_light, r_normal);
        float specular = pow(max(dot(reflected_light, -ray_direction), 0.0), specular_power_val) * L_attenuation; 
        final_color += specular * specular_strength_val; 

        // Apply flashIntensity as a global brightness multiplier for the lit fractal
        // This replaces the "ugly circle" and provides overall light control
        final_color *= flashIntensity;

        // Apply glitch effect if enabled
        if (glitchAmount > 0.0)
            final_color += sin(p_hit.z * 25. + t * 10.) * glitchAmount;

        // Apply fog based on distance
        const float fog_density = 3.0; // Default value from reference (not an ISF input)
        final_color *= exp(-d / fog_density);
        
        // Apply vignette
        final_color -= dot(-u, u) * vignetteStrength; 
        
        // Apply post-processing (brightness, saturation, contrast, and color pulse value)
        final_color = post(final_color, colorPulseValue(t));
        
        // Final tone mapping to prevent over-exposure
        final_color = final_color / (final_color + 0.155) * 1.019;

        // Hardcoded final adjustments (not in JSON inputs)
        const float gamma_correction_val = 1.2;
        const float output_gain_val = 1.7;
        const float dithering_enabled_val = 1.0;

        // Apply gamma correction and output gain
        final_color = pow(final_color, vec3(gamma_correction_val)) * output_gain_val;

        // Apply dithering for smoother gradients if enabled.
        const highp float NOISE_GRANULARITY = 0.5 / 255.0; 
        float random_dither_val = fract(sin(dot(gl_FragCoord.xy, vec2(12.9898, 78.233))) * 43758.5453);
        if (dithering_enabled_val > 0.5) { 
            final_color += mix(-NOISE_GRANULARITY, NOISE_GRANULARITY, random_dither_val);
        }

    } else {
        // If no hit, render background (fixed black)
        final_color = vec3(0.0); 
        // light_halo_strength is hardcoded to 0.0, so no background light circle
        const float light_halo_strength_val = 0.0; // Default to off
        final_color += exp2(-length(cross(ray_direction, getLightPosition() - ro))) * light_halo_strength_val; 

        // Apply gamma correction and output gain to background color as well
        const float gamma_correction_val = 1.2;
        const float output_gain_val = 1.7;
        final_color = pow(final_color, vec3(gamma_correction_val)) * output_gain_val;
    }

    gl_FragColor = vec4(final_color, 1.0); // Set alpha to 1.0
}