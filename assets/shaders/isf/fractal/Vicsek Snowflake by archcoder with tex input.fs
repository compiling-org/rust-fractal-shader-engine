/*
{
  "DESCRIPTION": "Vicsek Snowflake Fractal converted to ISF, with controls for Ambient Occlusion, Vignette, Glow, Fog, Shadows, Lighting, Rendering, Coloring, Refraction, new manual fractal controls, and image textures.",
  "CATEGORIES": [ "Fractal", "Snowflake", "3D", "Procedural", "Abstract", "Texture" ],
  "ISF_VERSION": "2.0",
  "INPUTS": [
    { "NAME": "inputTex1", "TYPE": "image", "DESCRIPTION": "First image texture for surface coloring." },
    { "NAME": "inputTex2", "TYPE": "image", "DESCRIPTION": "Second image texture for surface coloring (currently unused, but available)." },
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Overall animation speed multiplier." },
    { "NAME": "ambient_occlusion_steps", "TYPE": "float", "DEFAULT": 6.0, "MIN": 1.0, "MAX": 10.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for Ambient Occlusion calculation." },
    { "NAME": "ambient_occlusion_radius", "TYPE": "float", "DEFAULT": 0.165, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius for Ambient Occlusion sampling." },
    { "NAME": "ambient_occlusion_darkness", "TYPE": "float", "DEFAULT": 0.37, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of Ambient Occlusion darkening." },
    { "NAME": "vignette_strength", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength of the vignette effect." },
    { "NAME": "vignette_radius", "TYPE": "float", "DEFAULT": 0.925, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Radius of the vignette effect." },
    { "NAME": "glow_intensity", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 5.0, "DESCRIPTION": "Intensity of the glow effect." },
    { "NAME": "glow_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the glow color." },
    { "NAME": "glow_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the glow color." },
    { "NAME": "glow_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the glow color." },
    { "NAME": "glow_threshold", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 100.0, "DESCRIPTION": "Distance threshold for glow activation." },
    { "NAME": "glow_falloff", "TYPE": "float", "DEFAULT": 0.9, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Falloff rate of the glow effect." },
    { "NAME": "super_glow", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable super glow (0=Off, 1=On)." },
    { "NAME": "glow_enabled", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable overall glow (0=Off, 1=On)." },
    { "NAME": "fog_color_r", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of the fog color." },
    { "NAME": "fog_color_g", "TYPE": "float", "DEFAULT": 0.6, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of the fog color." },
    { "NAME": "fog_color_b", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of the fog color." },
    { "NAME": "fog_density", "TYPE": "float", "DEFAULT": 0.08, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Density of the fog." },
    { "NAME": "fog_falloff", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Falloff rate of the fog." },
    { "NAME": "self_shadow_bias", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Bias for self-shadowing to prevent artifacts." },
    { "NAME": "shadow_darkness", "TYPE": "float", "DEFAULT": 0.2, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Darkness multiplier for shadows." },
    { "NAME": "shadow_steps", "TYPE": "float", "DEFAULT": 15.0, "MIN": 1.0, "MAX": 100.0, "STEP": 1.0, "DESCRIPTION": "Number of steps for shadow raymarching." },
    { "NAME": "shadow_softness", "TYPE": "float", "DEFAULT": 64.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Softness of the shadows." },
    { "NAME": "min_step_size", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 0.1, "DESCRIPTION": "Minimum step size for shadow raymarching." },
    { "NAME": "light_intensity", "TYPE": "float", "DEFAULT": 3.600001, "MIN": 0.0, "MAX": 50.0, "DESCRIPTION": "Overall intensity of lights." },
    { "NAME": "light1_position_x", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "X position of Light 1." },
    { "NAME": "light1_position_y", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Y position of Light 1." },
    { "NAME": "light1_position_z", "TYPE": "float", "DEFAULT": 10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Z position of Light 1." },
    { "NAME": "light2_position_x", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "X position of Light 2." },
    { "NAME": "light2_position_y", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Y position of Light 2." },
    { "NAME": "light2_position_z", "TYPE": "float", "DEFAULT": -10.0, "MIN": -50.0, "MAX": 50.0, "DESCRIPTION": "Z position of Light 2." },
    { "NAME": "light1_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of Light 1 color." },
    { "NAME": "light1_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of Light 1 color." },
    { "NAME": "light1_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of Light 1 color." },
    { "NAME": "light2_color_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of Light 2 color." },
    { "NAME": "light2_color_g", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of Light 2 color." },
    { "NAME": "light2_color_b", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of Light 2 color." },
    { "NAME": "iterations", "TYPE": "float", "DEFAULT": 9.0, "MIN": 1.0, "MAX": 50.0, "STEP": 1.0, "DESCRIPTION": "Number of fractal iterations." },
    { "NAME": "max_steps", "TYPE": "float", "DEFAULT": 120.0, "MIN": 1.0, "MAX": 500.0, "STEP": 1.0, "DESCRIPTION": "Maximum raymarch steps." },
    { "NAME": "ambient_light", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Ambient light contribution." },
    { "NAME": "max_distance", "TYPE": "float", "DEFAULT": 20.0, "MIN": 1.0, "MAX": 100.0, "DESCRIPTION": "Maximum raymarch distance." },
    { "NAME": "surface_distance", "TYPE": "float", "DEFAULT": 0.004, "MIN": 0.00001, "MAX": 0.1, "DESCRIPTION": "Distance to consider a surface hit." },
    { "NAME": "raystep_multiplier", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "DESCRIPTION": "Multiplier for raymarch step size." },
    { "NAME": "colors_enabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable palette coloring (0=Off, 1=On)." },
    { "NAME": "palette_color1_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of first palette color." },
    { "NAME": "palette_color1_g", "TYPE": "float", "DEFAULT": 0.7, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of first palette color." },
    { "NAME": "palette_color1_b", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of first palette color." },
    { "NAME": "palette_color2_r", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of second palette color." },
    { "NAME": "palette_color2_g", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of second palette color." },
    { "NAME": "palette_color2_b", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of second palette color." },
    { "NAME": "bg_color_r", "TYPE": "float", "DEFAULT": 0.05, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Red component of background color." },
    { "NAME": "bg_color_g", "TYPE": "float", "DEFAULT": 0.02, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Green component of background color." },
    { "NAME": "bg_color_b", "TYPE": "float", "DEFAULT": 0.01, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Blue component of background color." },
    { "NAME": "refraction_intensity", "TYPE": "float", "DEFAULT": 2.611, "MIN": 0.0, "MAX": 10.0, "DESCRIPTION": "Intensity of refraction/specular highlight." },
    { "NAME": "refraction_sharpness", "TYPE": "float", "DEFAULT": 8.0, "MIN": 1.0, "MAX": 30.0, "DESCRIPTION": "Sharpness of refraction/specular highlight." },
    { "NAME": "vicsek_scale", "TYPE": "float", "DEFAULT": 3.0, "MIN": 1.0, "MAX": 5.0, "DESCRIPTION": "Scale factor for the Vicsek fractal." },
    { "NAME": "vicsek_offset_x", "TYPE": "float", "DEFAULT": 1.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "X offset for Vicsek fractal transformation." },
    { "NAME": "vicsek_offset_y", "TYPE": "float", "DEFAULT": 0.0, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Y offset for Vicsek fractal transformation." },
    { "NAME": "vicsek_offset_z", "TYPE": "float", "DEFAULT": -0.4, "MIN": -5.0, "MAX": 5.0, "DESCRIPTION": "Z offset for Vicsek fractal transformation." },
    { "NAME": "camera_orbit_radius", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 20.0, "DESCRIPTION": "Radius of the camera's orbital path." },
    { "NAME": "camera_orbit_speed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.1, "MAX": 5.0, "DESCRIPTION": "Speed of the camera's orbital movement." },
    { "NAME": "zoom_factor", "TYPE": "float", "DEFAULT": 4.5, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Camera zoom level (higher value means more zoomed in)." },
    { "NAME": "texture_enabled", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "STEP": 1.0, "DESCRIPTION": "Enable/disable image texture (0=Off, 1=On)." },
    { "NAME": "texture_scale", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 10.0, "DESCRIPTION": "Scale of the applied texture." },
    { "NAME": "texture_blend_strength", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "DESCRIPTION": "Strength to blend the texture with the palette color." }
  ]
}
*/

// Define constants
#define GOLDEN_RATIO 1.6180339887498948482
#define PI 3.14 // Changed PI to 3.14




// Function Prototypes (forward declarations)
float cube_sdf3d(vec3 p, vec3 s);
vec2 vicseksnowflake_sdf(vec3 z);
vec3 ray_marcher(vec3 ro, vec3 rd);
float soft_shadow(vec3 p, vec3 light_pos, float k);
vec3 get_light(vec3 p, vec3 rd, vec3 ro, vec3 light_pos, vec3 light_color, vec3 normal);
vec3 calculate_normal(vec3 p);
float calculate_ambient_occlusion(vec3 p, vec3 normal);
vec3 tex3D_color(sampler2D tex, vec3 p, vec3 n); // New function for 3D texture sampling
vec3 render(vec3 ray_origin, vec3 ray_dir, vec2 screen_uv);
mat3 setCamera( in vec3 ro, in vec3 ta, float cr );


// SDF for a cube - directly from your code
float cube_sdf3d(vec3 p, vec3 s) { 
    vec3 q = abs(p) - s; 
    return length(max(q, 0.0)); 
}

// Vicsek Snowflake SDF function - directly from your code, adapted for ISF inputs
vec2 vicseksnowflake_sdf(vec3 z) {
    // All these variables are implicitly passed from the JSON INPUTS as floats
    // float vicsek_scale;
    // float vicsek_offset_x, vicsek_offset_y, vicsek_offset_z;
    // float iterations;

    float scale = vicsek_scale;
	vec3 offset = vec3(vicsek_offset_x, vicsek_offset_y, vicsek_offset_z); // Use new manual offsets
	float orbit_trap = 100000.0;
	float r;
    float s = 1.0;
    float d = 1000.0;
    
    z *= 2.0;

	int i = 0;
    for (; i < int(iterations); i++) { // Cast iterations to int
        z = abs(z) - 0.1;
        z = abs(z) + 0.1;
        if (z.x - z.y < 0.0) z.xy = z.yx;
        if (z.x - z.z < 0.0) z.xz = z.zx;
        if (z.y - z.z < 0.0) z.yz = z.zy;
        z = clamp(z, 0.0, 1.0) * 1.9 - z;
        
        vec3 p = z;
        if (p.x + p.y > 1.0) p.xy = p.yx;
        if (p.x + p.z > 1.0) p.xz = p.zx;
        if (p.y + p.z < 1.0) p.yz = p.zy;
        z = p;

		z = z * scale - offset * (scale - 1.0);
        s /= scale;

		if (z.y > 0.5) z.y -= 1.0;
        z.x += 1.0;
        
		r = dot(z, z);
		orbit_trap = min(orbit_trap, r);
	}
    
    d = cube_sdf3d(z, vec3(0.5)) * s;
	return vec2(d / 2.5, orbit_trap).yx; // Swizzle to match original return order
}

// Raymarcher function - directly from your code
vec3 ray_marcher(vec3 ro, vec3 rd) {
    // Implicitly passed float inputs: max_steps, raystep_multiplier, surface_distance, max_distance, super_glow
	float dfo = 0.0;
	float orbit_trap_distance;
	float total_marches = 0.0;

	for (int i = 0; i < int(max_steps); i++) { // Cast max_steps to int
		vec2 data = vicseksnowflake_sdf(ro + rd * dfo);
		float point_distance = data.y;
		dfo += point_distance * raystep_multiplier;
		total_marches += 1.0;

		if (abs(point_distance) < surface_distance || dfo > max_distance) {
			orbit_trap_distance = data.x;
			break;
		}
	}

	if (super_glow > 0.5 && dfo < max_distance) { total_marches = float(max_steps); } // Check super_glow as float

	return vec3(dfo > max_distance ? 0.0 : orbit_trap_distance, dfo, total_marches);
}

// Soft Shadow function - directly from your code
float soft_shadow(vec3 p, vec3 light_pos, float k) {
    // Implicitly passed float inputs: surface_distance, self_shadow_bias, shadow_steps, min_step_size, max_distance
	vec3 rd = normalize(light_pos - p);
	float res = 1.0;
	float ph = 1e20;
	float t = surface_distance + self_shadow_bias;

	for (int i = 0; i < int(shadow_steps); i++) { // Cast shadow_steps to int
		float h = vicseksnowflake_sdf(p + rd * t).y;

		if (h < surface_distance) {
			return 0.0;
		}

		float y = h * h / (2.0 * ph);
		float d = sqrt(h * h - y * y);
		res = min(res, k * d / max(0.0, t - y));
		ph = h;

		t += max(h, min_step_size);

		if (t >= max_distance) {
			break;
		}
	}

	return clamp(res, 0.0, 1.0);
}

// Get Light function - directly from your code
vec3 get_light(vec3 p, vec3 rd, vec3 ro, vec3 light_pos, vec3 light_color, vec3 normal) {
    // Implicitly passed float inputs: light_intensity, ambient_light, shadow_darkness, shadow_softness, refraction_intensity, refraction_sharpness
	vec3 to_light = normalize(light_pos - p);
	float light = light_intensity * clamp(dot(to_light, normal), 0.05, 1.0);

	float shadow = soft_shadow(p, light_pos, shadow_softness);
	light *= max(shadow, shadow_darkness);
	vec3 reflection = reflect(to_light, normal);
	float specular = pow(max(dot(reflection, rd), 0.0), refraction_sharpness);
	light *= max(specular * refraction_intensity, 1.0);

	return max(light_color * light, ambient_light);
}

// Calculate Normal function - directly from your code
vec3 calculate_normal(vec3 p) {
	float h = 0.000001;
	return normalize(vec3(
		vicseksnowflake_sdf(p + vec3(h, 0.0, 0.0)).y - vicseksnowflake_sdf(p - vec3(h, 0.0, 0.0)).y,
		vicseksnowflake_sdf(p + vec3(0.0, h, 0.0)).y - vicseksnowflake_sdf(p - vec3(0.0, h, 0.0)).y,
		vicseksnowflake_sdf(p + vec3(0.0, 0.0, h)).y - vicseksnowflake_sdf(p - vec3(0.0, 0.0, h)).y
	));
}

// Calculate Ambient Occlusion function - directly from your code
float calculate_ambient_occlusion(vec3 p, vec3 normal) {
    // Implicitly passed float inputs: ambient_occlusion_steps, ambient_occlusion_radius, ambient_occlusion_darkness
	float occlusion = 0.0;
	float weight = 1.0 / float(ambient_occlusion_steps);

	for (int i = 0; i < int(ambient_occlusion_steps); i++) { // Cast to int
		float ao_scale = float(i + 1) / float(ambient_occlusion_steps);
		vec3 sample_point = p + normal * ao_scale * ambient_occlusion_radius;
		float d = vicseksnowflake_sdf(sample_point).y;
		occlusion += max(ambient_occlusion_radius - d, 0.0) * weight / ambient_occlusion_radius;
	}

	return 1.0 - clamp(occlusion, 0.0, 1.0) * ambient_occlusion_darkness; // Apply darkness
}

// New function to sample 3D texture and return color
vec3 tex3D_color(sampler2D tex, vec3 p, vec3 n) {
    vec3 nn = max((abs(n) - 0.2) * 7., 0.001);
    nn /= (nn.x + nn.y + nn.z);
    vec2 uv1 = fract(vec2(p.y, p.z));
    vec2 uv2 = fract(vec2(p.z, p.x));
    vec2 uv3 = fract(vec2(p.x, p.y));
    
    vec3 c1 = texture2D(tex, uv1).rgb;
    vec3 c2 = texture2D(tex, uv2).rgb;
    vec3 c3 = texture2D(tex, uv3).rgb;

    return c1 * nn.x + c2 * nn.y + c3 * nn.z;
}

// Render function - directly from your code, adapted for ISF inputs
vec3 render(vec3 ray_origin, vec3 ray_dir, vec2 screen_uv) {
    // Implicitly passed float inputs: max_distance, vignette_radius, vignette_strength, colors_enabled, glow_enabled, glow_threshold, glow_intensity, glow_falloff, fog_density, fog_falloff
	vec3 data = ray_marcher(ray_origin, ray_dir);
	float orbit_trap = data.x;
	float dfo = data.y;
	float total_marches = data.z;

    // Reconstruct vec3 colors and positions from float inputs
    vec3 reconstructed_palette_color1 = vec3(palette_color1_r, palette_color1_g, palette_color1_b);
    vec3 reconstructed_palette_color2 = vec3(palette_color2_r, palette_color2_g, palette_color2_b);
    vec3 reconstructed_bg_color = vec3(bg_color_r, bg_color_g, bg_color_b);
    vec3 reconstructed_glow_color = vec3(glow_color_r, glow_color_g, glow_color_b);
    vec3 reconstructed_fog_color = vec3(fog_color_r, fog_color_g, fog_color_b);
    vec3 reconstructed_light1_color = vec3(light1_color_r, light1_color_g, light1_color_b);
    vec3 reconstructed_light2_color = vec3(light2_color_r, light2_color_g, light2_color_b);
    vec3 reconstructed_light1_position = vec3(light1_position_x, light1_position_y, light1_position_z);
    vec3 reconstructed_light2_position = vec3(light2_position_x, light2_position_y, light2_position_z);

    // Use colors_enabled to conditionally mix palette colors
	vec3 palette_col = mix(reconstructed_palette_color1, reconstructed_palette_color2, mix(0.0, orbit_trap, colors_enabled));
	vec3 final_color_base;

	if (dfo >= max_distance) {
		float vignette = smoothstep(vignette_radius, vignette_radius - vignette_strength, length(screen_uv - vec2(0.5)));
		final_color_base = reconstructed_bg_color * vignette;
	} else {
        vec3 p = ray_origin + ray_dir * dfo;
        vec3 normal = calculate_normal(p);

        float ao = max(calculate_ambient_occlusion(p, normal), 0.0);
        vec3 light1 = get_light(p, ray_dir, ray_origin, reconstructed_light1_position, reconstructed_light1_color, normal);
        vec3 light2 = get_light(p, ray_dir, ray_origin, reconstructed_light2_position, reconstructed_light2_color, normal);

        // Sample texture color
        vec3 sampled_texture_color = tex3D_color(inputTex1, p * texture_scale, normal);
        
        // Blend palette color with texture color based on texture_enabled and texture_blend_strength
        vec3 surface_color = mix(palette_col, sampled_texture_color, texture_blend_strength * texture_enabled);

        float vignette = smoothstep(vignette_radius, vignette_radius - vignette_strength, length(screen_uv - vec2(0.5)));
        final_color_base = surface_color * ao * (light1 + light2) * vignette;
	}

    vec3 final_color = final_color_base; // Start with the base color

	if (glow_enabled > 0.5 && float(total_marches) * raystep_multiplier > glow_threshold) { // Check glow_enabled as float
		float final_glow_intensity = (glow_intensity - 0.2) * smoothstep(glow_threshold, 100.0, float(total_marches) * raystep_multiplier);
		vec3 final_glow_color = reconstructed_glow_color * 3.0;
		final_color += final_glow_color * pow(final_glow_intensity, glow_falloff);
	}

	float fog_distance = dfo < max_distance ? dfo : max_distance;
	float fog_amount = 1.0 - exp(-fog_density * fog_distance);
	final_color = mix(final_color, reconstructed_fog_color, pow(fog_amount, fog_falloff));

	return final_color;
}

// Set Camera function - directly from your code
mat3 setCamera( in vec3 ro, in vec3 ta, float cr )
{
	vec3 cw = normalize(ta-ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv =          ( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void main() { // ISF entry point
    vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
    // float time = iTime*0.8; -> adapted to ISF TIME and speed input
	float time = TIME * camera_orbit_speed; // Use ISF TIME and camera_orbit_speed

    vec3 ta = vec3( 0.0, 0.0, 0.0 );
    // Camera position using ISF inputs for orbit
    vec3 ro = ta + vec3(camera_orbit_radius * cos(time), camera_orbit_radius * sin(time), camera_orbit_radius * cos(time));
    mat3 ca = setCamera( ro, ta, 0.0 ); // cr is 0.0 in original

    vec2 p = (2.0*gl_FragCoord.xy-RENDERSIZE.xy)/RENDERSIZE.y;
    vec3 rd = ca * normalize(vec3(p, zoom_factor)); // Use zoom_factor input
    vec3 col = render(ro, rd, uv);

    col = col * 3.0 / (2.5 + col);
    col = pow( col, vec3(0.4545) );
    
    gl_FragColor = vec4(col, 1.0);
}