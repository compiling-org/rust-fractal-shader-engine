/*
{
  "CATEGORIES": [
    "Automatically Converted",
    "Shadertoy"
  ],
  "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/Wcd3W2 by Shane. Following a glowing spiral through a lava tube tunnel, with added tunable parameters for customization.",
  "IMPORTED": {
  },
  "INPUTS": [
    { "NAME": "TunnelSpeed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Tunnel Speed" },
    { "NAME": "MainGlowIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Main Glow Intensity" },
    { "NAME": "RockColorMix", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Rock Color Blend" },
    { "NAME": "TunnelRadius", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.5, "MAX": 2.0, "LABEL": "Tunnel Radius Scale" },
    { "NAME": "SpiralThickness", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 2.0, "LABEL": "Spiral Thickness Scale" },
    { "NAME": "CameraOrbitStrength", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "Camera Orbit Strength" },
    { "NAME": "MainLightIntensity", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0, "LABEL": "Main Light Intensity" },


    { "NAME": "PaletteMode", "TYPE": "long", "DEFAULT": 0, "MIN": 0, "MAX": 3, "LABEL": "Color Palette Mode", "LABELS": ["Rainbow Flow", "Complementary Pulse", "Triadic Harmony", "Acid Trip Glitch"] },
    { "NAME": "PaletteBlendSpeed", "TYPE": "float", "DEFAULT": 0.8, "MIN": 0.0, "MAX": 3.0, "LABEL": "Palette Blend Speed" },
    { "NAME": "HueShiftSpeed", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 5.0, "LABEL": "Global Hue Shift Speed" },
    { "NAME": "PulseHueAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Hue Amplitude" },
    { "NAME": "PulseSaturationAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Saturation Amplitude" },
    { "NAME": "PulseValueAmplitude", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "Pulse Value Amplitude" }
  ]
}
*/


/*
    Glowing Spiral Lava Tunnel
    --------------------------

    The other day, Xor posted a beautiful example consisting of a glowing
    spiral running down a square tube. It's a reasonably common setup, but
    Xor managed to code it up in just a few lines whilst giving it a kind of
    path traced look, which was pretty amazing. The link is below, for anyone
    who hasn't seen it.

    Anyway, I repurposed some old rocky tunnel code with the glowing spiral
    in mind, and this is the result. It's not super exciting, but I wanted to
    get my 300th shader on the board. Hooray! Only 2000 more to catch up to
    Fabrice. :D

    The distance function was put together some time ago without a lot of
    pre-planning, so it could do with some rearranging and fine tuning. The
    frame rate on my machine is acceptable, but I know some things that could
    improve it, which I'll endeavor to put into effect in due course.



    Other examples:


    // A beautiful result with a confusingly small character count. :)
    Corridor [334] -- Xor
    https://www.shadertoy.com/view/tf33Wf

    // Diatribes has been doing a few tunnel examples lately. I liked this one.
    DDA Gem Cruise -- diatribes
    https://www.shadertoy.com/view/wXj3Wt

    // In pixelshader time, this was coded a gazillion years ago, and
    // is still cool to look at.
    Tissue -- iq
    https://www.shadertoy.com/view/XdBSzd

    // Better usage of the cellular algorithm and XT95's translucency formula.
    3D Cellular Tiling -- Shane
    https://www.shadertoy.com/view/ld3Szs


*/


 // Surface structure: There are 4, but virtually anything will work.
// Cellular: 0, Gyroid: 1, Gyroid with 2 levels: 2, Gyroid (reverse space): 3.
#define SURFACE 1


////////////////////
// 2 PI.
#define TAU 6.2831853
// Far plane.
#define FAR 50.

// Loop... anti-unrolling hack. :)
// Removed FRAMEINDEX dependency for broader ISF compatibility.
// #define ZERO min(int(FRAMEINDEX), 0)
/////////////////////

// Standard 2D rotation formula.
mat2 rot2(in float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }


// A slight variation on one of Dave Hoskins's hash functions,
// which you can find here:
//
// Hash without Sine -- Dave Hoskins
// https://www.shadertoy.com/view/4djSRW
// 1 out, 2 in...
#define STATIC
float hash21(vec2 p){

  vec3 p3  = fract(vec3(p.xyx)*.1031);
  p3 += dot(p3, p3.yzx + 42.123);

  #ifdef STATIC
  return fract((p3.x + p3.y) * p3.z);
  #else
  p3.x = fract((p3.x + p3.y) * p3.z);
  return sin(p3.x*TAU + TIME); // Animation, if desired.
  #endif
}



// Standard 1x1 hash functions. Using "cos" for non-zero origin result.
float hash(float n){ return fract(cos(n)*45758.5453); }



// IQ's "uint" based uvec3 to float hash.
// This version is rewritten to avoid uvec3 and floatBitsToUint for broader compatibility.
// It's a float-based permutation hash.
float hash31(vec3 p) {
  p = fract(p * 0.3183099 + vec3(0.1));
  p = p * (p + 2.0);
  p.x += p.y; // Simplified dot product
  p.x += p.z;
  return fract(p.x * 0.94191);
}



// Commutative smooth maximum function. Provided by Tomkh, and taken
// from Alex Evans's (aka Statix) talk:
// http://media.lolrus.mediamolecule.com/AlexEvans_SIGGRAPH-2015.pdf
// Credited to Dave Smith @media molecule.
float smax(float a, float b, float k){

  float f = max(0., 1. - abs(b - a)/k);
  return max(a, b) + k*.25*f*f;
}


// Commutative smooth minimum function. Provided by Tomkh, and taken
// from Alex Evans's (aka Statix) talk:
// http://media.lolrus.mediamolecule.com/AlexEvans_SIGGRAPH-2015.pdf
// Credited to Dave Smith @media molecule.
float smin(float a, float b, float k){

  float f = max(0., 1. - abs(b - a)/k);
  return min(a, b) - k*.25*f*f;
}

// 2D vector version.
vec2 smin(vec2 a, vec2 b, float k){

  vec2 f = max(vec2(0), 1. - abs(b - a)/k);
  return min(a, b) - k*.25*f*f;
}

// More concise, self contained version of IQ's original 3D noise function.
float noise3D(in vec3 p){

  // Just some random figures, analogous to stride. You can change this, if you want.
  const vec3 s = vec3(113, 157, 1);

  vec3 ip = floor(p); // Unique unit cell ID.

  // Setting up the stride vector for randomization and interpolation, kind of.
  // All kinds of shortcuts are taken here. Refer to IQ's original formula.
  vec4 h = vec4(0., s.yz, s.y + s.z) + dot(ip, s);

  p -= ip; // Cell's fractional component.

  // A bit of cubic smoothing, to give the noise that rounded look.
  p = p*p*(3. - 2.*p);

  // Standard 3D noise stuff. Retrieving 8 random scalar values for each cube corner,
  // then interpolating along X. There are countless ways to randomize, but this is
  // the way most are familar with: fract(sin(x)*largeNumber).
  h = mix(fract(sin(mod(h, TAU))*43758.5453),
          fract(sin(mod(h + s.x, TAU))*43758.5453), p.x);

  // Interpolating along Y.
  h.xy = mix(h.xz, h.yw, p.y);

  // Interpolating along Z, and returning the 3D noise value.
  return mix(h.x, h.y, p.z); // Range: [0, 1].

}

////////
// The cellular tile routine. Draw a few objects (four spheres, in this case) using a minumum
// blend at various 3D locations on a cubic tile. Make the tile wrappable by ensuring the
// objects wrap around the edges. That's it.
//
// Believe it or not, you can get away with as few as three spheres. If you sum the total
// instruction count here, you'll see that it's way, way lower than 2nd order 3D Voronoi.
// Not requiring a hash function provides the biggest benefit, but there is also less setup.
//
// The result isn't perfect, but 3D cellular tiles can enable you to put a Voronoi looking
// surface layer on a lot of 3D objects for little cost.
//
float drawObject(in vec3 p){

  // Wrap conditions:
  // Anything that wraps the domain will work.
  //p = cos(p*6.2831853)*.25 + .25;
  //p = abs(cos(p*3.14159)*.5);
  //p = fract(p) - .5;
  //p = abs(fract(p) - .5);

  // Distance metrics:
  // Here are just a few variations. There are way too many to list them all,
  // but you can try combinations with "min," and so forth, to create some
  // interesting combinations.

  // Spherical.
  p = fract(p) - .5;
  return dot(p, p);

  // Octahedral... kind of.
  //p = abs(fract(p)-.5);
  //return dot(p, vec3(.333)) - .05;

  // Triangular tube - Doesn't wrap, but it's here for completeness.
  //p = fract(p) - .5;
  //p = max(abs(p)*.866025 + p.yzx*.5, -p.yzx);
  //return max(max(p.x, p.y), p.z) - .175;

  // Cubic.
  //p = abs(fract(p) - .5);
  //return max(max(p.x, p.y), p.z) - .15;

  // Cylindrical. (Square root needs to be factored to "d" in the cellTile function.)
  //p = fract(p) - .5;
  //return max(max(dot(p.xy, p.xy), dot(p.yz, p.yz)), dot(p.xz, p.xz));

  // Octahedral.
  //p = abs(fract(p) - .5);
  //p += p.yzx;
  //return max(max(p.x, p.y), p.z)*.5;

  // Hexagonal tube.
  //p = abs(fract(p) - .5);
  //p = max(p*.866025 + p.yxy*.5, p.yxy); // Corrected swizzle
  //return max(max(p.x, p.y), p.z);


}



// Second order cellular tiled routine - I've explained how it works in detail in other
// examples.
float cellTile(in vec3 p){


  // Draw four overlapping objects (spheres, in this case) at various positions
  // throughout the tile. Hand coding the positions cuts down on calculation.
  vec4 v_dists, d_val; // Renamed to avoid `d` collision with `d.x` etc.
  v_dists.x = drawObject(p - vec3(.81, .62, .53));
  p.xy = vec2(p.y - p.x, p.y + p.x)*.7071;
  v_dists.y = drawObject(p - vec3(.39, .2, .11));
  p.yz = vec2(p.z - p.y, p.z + p.y)*.7071;
  v_dists.z = drawObject(p - vec3(.62, .24, .06));
  p.xz = vec2(p.z - p.x, p.z + p.x)*.7071;
  v_dists.w = drawObject(p - vec3(.2, .82, .64));

  // Second order.
  //v.xy = min(d.xz, d.yw), v.z = min(max(d.x, d.y),
  //          max(d.z, d.w)), v.w = max(v.x, v.y);
  //d.x =  min(v.z, v.w) - min(v.x, v.y); // First minus second order. Range [0, 1].

  // First order.
  d_val.x = smin(smin(v_dists.x, v_dists.z, .05), smin(v_dists.y, v_dists.w, .05), .05); // Fixed for scalar smin

  return d_val.x; // Return the distance.

}

// The path is a 2D sinusoid that varies over time, depending upon the frequencies,
// and amplitudes.
vec2 path(in float z){

  //return vec2(0); // Straight line.

  // Curved path.
  float a = sin(z*.11);
  float b = cos(z*.14);
  return vec2(a*4. - b*1.5, b*1.7 + a*1.5);
}

// IQ's 2D box function, with added smoothing factor.
float sBoxS(in vec2 p, in vec2 b, in float rf){

  vec2 d = abs(p) - b + rf;
  return min(max(d.x, d.y), 0.) + length(max(d, 0.)) - rf;

}


// Tube functions - for the tunnel.
float dist2D(in vec2 p){

  // Other tube shapes to try.
  /*
  p = abs(p);
  return max(max(p.x, p.y), (p.x + p.y)*.7071); // Octagon.
  return max(p.x, p.y); // Square.
  return (p.x + p.y)*.7071; // Diamond.
  return max(p.x*.8660254 + p.y*.5, p.y); // Hexagon.
  */

  return length(p); // Round cylinder.

}

// Surface ID and glow.
int gID;
vec3 glow;


// Converts HSV (Hue, Saturation, Value) color to RGB
vec3 hsv2rgb(vec3 c) {
  vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.w);
  return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


// Perturbed gyroid (or cellular) tunnel function: In essence, it's one or two
// smoothly combined gyroid functions, with a cylindrical hole (wrapped around the
// camera path) smoothly carved out from them.
//
float map(vec3 p){

  // Trancendental gyroid or cellular functions with extra functions to perturb
  // the tunnel.
  //
  // As and aside, I hadn't looked at this example for ages, so I had to spend
  // a while trying to figure out what all this mess was... And that is why you
  // should comment as you go. :D


  // Surfaces.
  #if SURFACE > 1
  vec3 q = p*3.14159265;
  float cav = dot(cos(q/2.), sin(q.yzx/2.)); // Gyroid one.
  float cav2 = dot(cos(q/4.), sin(q.yzx/4.)); // Gyroid two.
  cav = smin(cav, cav2/2., 1.); // Smoothly combine the gyroids.
  q = p;
  #else
  vec3 q = p;
  #endif

  // Extra cellular functions to create the crater like surface.
  float cav3 = (cellTile(q/2.))*2. - .08;
  float cav4 = (cellTile(q*1.5 + .5))/1.5 - .01;


  // Cellular based surface distance.
  #if SURFACE == 0
  float sD = (.05 - cellTile(p/6.))*6.;
  #endif

  // Wrap the tunnel, and anything that follows, around the camera path.
  vec2 pth = path(p.z);
  p.xy -= pth;


  // Gyroid based surfaces.
  #if SURFACE == 1
  float sD = .5 - abs(dot(cos((p)*TAU/8.), sin(p.yzx*TAU/8.))); // Single gyroid.
  #elif SURFACE == 2
  float sD = cav + .2; // Two combined gyroids.
  #elif SURFACE == 3
  float sD = -cav - .6; // The rough reverse of the above.
  #endif

  // Smoothly carve out the tunnel.
  sD = smax((1. - dist2D(p.xy) * TunnelRadius), sD, 2.); // Use TunnelRadius

  // Using the original structure to create an inner one.
  // Glowing inner surface.
  float innerSD = sD + .85;
  //innerSD = abs(innerSD + .05) - .05; // Giving it shell thickness.
  sD = smax(sD, -(innerSD - .1), .15);

  // Moon-like surface crates.
  sD = smax(sD, -cav3, .35);
  sD = smax(sD, -cav4, .15);




  // Coordinates.
  vec3 q3 = p;
  float tunSc = .95;

  // Spiraling the coordinates around the XY plane along the Z coordinates.
  q3.xy *= rot2(p.z/2.);
  //vec2 sgn = sign(q3.xy);
  // Perturbing slightly to match the gyroid walls... I need to look
  // this over later.
  q3.xy += (dot(cos(q3*TAU/8.), sin(q3.yzx*TAU/8.))*.125 + .125);




  // Giving the glowing cable light path some radius.
  q3.xy -= tunSc;
  // Repeating across the diagonal... There'd be a better way to do this.
  // Comment out the line above first,or they won't be centered.
  //q3.xy = (vec2(q3.y + abs(q3.x), abs(q3.x) - q3.y) - tunSc)/sqrt(2.);
  //q3.xy = rot2(-TAU/8.)*vec2(abs(q3.x), q3.y) - tunSc/sqrt(2.);



  // Creating the spiraling cable, then adding it to the surface, since
  // we'll be using the same materials. Normally though, you should keep
  // it seperate from the other objects.
  float cable = dist2D(q3.xy + .025 * SpiralThickness) - .025 * SpiralThickness; // Use SpiralThickness
  sD = min(sD, cable);


  // Repeat space for the repeat lights that run allong the cable..

  // Rescaling, for the ball joints.
  float zSc = 1./2.;
  q3.z = p.z - zSc/2.*0. - TIME*0.;
  float ipZ = floor(q3.z/zSc);
  q3.z -= (ipZ + .5)*zSc;
  //float ball = length(q3  + .05) - .1;
  float lgt2D = sBoxS(q3.xy + .025 * SpiralThickness, vec2(.05 * SpiralThickness), .02 * SpiralThickness); // Use SpiralThickness
  float lgt = sBoxS(vec2(lgt2D, q3.z), vec2(.05 * SpiralThickness,  zSc/4.), .02 * SpiralThickness); // Use SpiralThickness
  // lgt = 1e5;  // Debug.
  //cable = 1e5;  // Debug.


  // Repeat spiral light glow.
  //
  // The hard coded numbers control the way the glow spreads through the scene,
  // and consequently, how much light falls on the surrounding surfaces, and
  // the reason for that is... something based in science that I've forgotten. :D
  float gA = 1./(.5 + lgt*lgt*32.);
  float rnd = hash(ipZ + .12);
  //vec3 gCol = .5 + .45*cos(TAU*rnd/4. + vec3(0, 1, 2)*1.3);
  //gCol *= vec3(gA, gA*sqrt(gA), gA*gA).yxz;
  vec3 gCol = vec3(gA, gA*sqrt(gA)*.4, gA*gA*.2)*2. * MainGlowIntensity; // Apply MainGlowIntensity
  if(lgt<innerSD){
    gCol = mix(gCol, gCol/5., 1. - smoothstep(.35, .45, sin(TAU*ipZ/16. - TIME*4.)*.5));

  }
  if(lgt<sD && lgt<1.) glow += gCol;

  // Encapsulated inner surface glow.
  gA = 1./(.5 + innerSD*innerSD*128.);
  gCol = vec3(gA, gA*sqrt(gA)*.4, gA*gA*.2) * MainGlowIntensity; // Apply MainGlowIntensity
  if((innerSD*innerSD/2.<sD) && innerSD<1.){
    glow += gCol/2.;
  }


  // Scene object IDs and minumum scene distance calculations.
  gID = sD<lgt && sD<innerSD? 0 : lgt<innerSD? 1 : 2;
  sD = min(sD, lgt);
  sD = min(sD, innerSD);

  // Return the distance value for the scene.
  return sD;

}


// Standard raymarching function.
float trace(in vec3 ro, in vec3 rd){

  // Reset the glow to zero.
  glow = vec3(0);

  // Note the jittering, since we're using cheap glow.
  float d, t = hash31(fract(ro*88.567)*7. + rd)*.25;
  for(int i = 0; i<128; i++){ // Replaced ZERO with 0

    // Surface distance.
    d = map(ro + rd*t);
    // Surface distance check.
    if(abs(d)<.001 || t>FAR) break; // Alternative: 0.001*max(t*.25, 1.)
    // Since we're calculatig glow inside the distance function (which is
    // a cheap hack), we need to delimit the ray jumping distance a bit.
    t += min(d*.9, .2);

  }

  // Clamp the distace to the far plane, in order to avoid occasional artifacts.
  return min(t, FAR);
}

// Normal function. It's not as fast as the tetrahedral calculation, but more symmetrical.
vec3 normal(in vec3 p) {

  //return normalize(vec3(m(p + e.xyy) - m(p - e.xyy), m(p + e.yxy) - m(p - e.yxy),
  //                      m(p + e.yyx) - m(p - e.yyx)));

  // This mess is an attempt to speed up compiler time by contriving a break... It's
  // based on a suggestion by IQ. I think it works, but I really couldn't say for sure.
  float sgn = 1.;
  vec3 e = vec3(.001, 0, 0), mp = e.zzz; // Spalmer's clever zeroing.
  for(int i = 0; i<6; i++){ // Replaced ZERO with 0
    mp.x += map(p + sgn*e)*sgn;
    sgn = -sgn;
    if(mod(float(i), 2.0) == 1.0){ mp = mp.yzx; e = e.zxy; } // Replaced bitwise AND
  }

  return normalize(mp);
}



// Cheap shadows are hard. In fact, I'd almost say, shadowing particular scenes with limited
// iterations is impossible... However, I'd be very grateful if someone could prove me wrong. :)
float softShadow(vec3 ro, vec3 lp, float k){

  // More would be nicer. More is always nicer, but not affordable for slower machines.
  const int iter = 32;

  vec3 rd = lp - ro; // Unnormalized direction ray.

  float shade = 1.;
  float t = 0.;
  float end = max(length(rd), 1e-5);
  rd /= end;


  // Max shadow iterations - More iterations make nicer shadows, but slow things down. Obviously, the lowest
  // number to give a decent shadow is the best one to choose.
  for (int i = 0; i<iter; i++){ // Replaced ZERO with 0

    float d = map(ro + rd*t);
    shade = min(shade, k*d/t);

    // Early exits.
    if (d<0. || t>end) break;

    //shade = min(shade, smoothstep(0., 1., k*h/dist)); // IQ's subtle refinement.
    t += clamp(d, .01, .5);



  }

  // Sometimes, I'll add a constant to the final shade value, which lightens the shadow a bit --
  // It's a preference thing. Really dark shadows look too brutal to me. Sometimes, I'll add
  // AO also just for kicks. :)
  return max(shade, 0.);
}




// I keep a collection of occlusion routines... OK, that sounded really nerdy. :)
// Anyway, I like this one. I'm assuming it's based on IQ's original.
float calcAO(in vec3 p, in vec3 n){

  float sca = 2., occ = 0.;
  for( int i = 0; i<5; i++ ){

    float hr = float(i + 1)*.15/5.;
    float d = map(p + n*hr);
    occ += (hr - d)*sca;
    sca *= .75;
  }

  return clamp(1. - occ, 0., 1.);
}


// Slightly modified version of Nimitz's curve function. The tetrahedral and normal six
// tap versions are in there. If four taps gives you what you want, then that'd be the
// one to use.
//
// I think it's based on a discrete finite difference approximation to the continuous
// Laplace differential operator? Either way, it gives you the curvature of a surface,
// which is pretty handy.
//
// Original usage (I think?) - Cheap curvature: https://www.shadertoy.com/view/Xts3WM
// Other usage: Xyptonjtroz: https://www.shadertoy.com/view/4ts3z2
//
// spr: sample spread, amp: amplitude, offs: offset.
float curve(in vec3 p, in float spr, in float amp, in float offs){

  float d = map(p);

  spr /= 450.;

  #if 0
  // Tetrahedral.
  vec2 e = vec2(-spr, spr); // Example: ef = .25;
  float d1 = map(p + e.yxx), d2 = map(p + e.xxy);
  float d3 = map(p + e.xyx), d4 = map(p + e.yyy);
  return clamp((d1 + d2 + d3 + d4 - d*4.)/e.y/2.*amp + offs + .5, 0., 1.);
  #else
  // Cubic.
  vec2 e = vec2(spr, 0); // Example: ef = .5;
  float d1 = map(p + e.xyy), d2 = map(p - e.xyy);
  float d3 = map(p + e.yxy), d4 = map(p - e.yxy);
  float d5 = map(p + e.yyx), d6 = map(p - e.yyx);

  #if 1
  //return clamp((d1 + d2 + d3 + d4 + d5 + d6 - d*6.)/e.x*amp + offs + .05, -.1, .1)/.1;
  return smoothstep(-.05, .05, (d1 + d2 + d3 + d4 + d5 + d6 - d*6.)/e.x/2.*amp + offs);

  #else
  d *= 2.;
  return 1. - smoothstep(-.05, .05, (abs(d1 + d2 - d) + abs(d3 + d4 - d) +
                                  abs(d5 + d6 - d))/e.x/2.*amp + offs + .0);
  #endif

  #endif

}


//////

void main() {
  // Use the global TunnelSpeed for TIME
    float time = TIME * TunnelSpeed;

    // Screen coordinates.
    vec2 uv = (gl_FragCoord.xy - RENDERSIZE.xy*.5)/RENDERSIZE.y;

    // Camera Setup.
    vec3 lookAt = vec3(0, 0, time*4.);  // "Look At" position.
    vec3 camPos = lookAt + vec3(0, 0, -.2); // Camera position, doubling as the ray origin.

    // Add camera orbit
    float orbitAngle = time * 0.1;
    vec2 orbitOffset = vec2(sin(orbitAngle), cos(orbitAngle)) * CameraOrbitStrength * 2.0;
    camPos.xy += orbitOffset;
    lookAt.xy += orbitOffset;

    // Light positioning.
    vec3 lightPos = camPos + vec3(0, .25, 5); // Placed in front of the camera.
    // Using the Z-value to perturb the XY-plane.
    // Sending the camera, "look at," and two light vectors down the tunnel. The "path" function is
    // synchronized with the distance function. Change to "path2" to traverse the other tunnel.
    lookAt.xy += path(lookAt.z);
    camPos.xy += path(camPos.z);
    lightPos.xy += path(lightPos.z);
    // Using the above to produce the unit ray-direction vector.
    float FOV = TAU/6.; // FOV - Field of view.
    vec3 forward = normalize(lookAt - camPos);
    vec3 right = normalize(vec3(forward.z, 0, -forward.x ));
    vec3 up = cross(forward, right);
    // rd - Ray direction.
    vec3 rd = normalize(uv.x*right + uv.y*up + forward/FOV );

    // A bit of lens mutation to increase the scene peripheral, if that's your thing.
    //vec3 rd = normalize(forward + FOV*uv.x*right + FOV*uv.y*up);
    //rd = normalize(vec3(rd.xy, rd.z - dot(rd.xy, rd.xy)*.25));

    // Swiveling the camera about the XY-plane (from left to right) when turning corners.
    // Naturally, it's synchronized with the path in some kind of way.
    rd.xy = rot2(-path(lookAt.z).x/16.)*rd.xy;

    // Standard ray marching routine. I find that some system setups don't like anything other than
    // a "break" statement (by itself) to exit.
    float t = trace(camPos, rd);

    // Object ID and glow.
    int svGID = gID;
    vec3 svGlow = glow;




    // Distance light source in the background.
    vec3 sky = vec3(1, .8, .55);
    //sky = mix(sky, sky.xzy, 315 - rd.y*.35);

    vec3 gP = camPos + rd*t;
    vec2 pth = gP.xy - path(gP.z);
    svGlow = mix(svGlow.xzy, svGlow, .5 + clamp(pth.y/.2, 0., 1.)*.5);
    sky = mix(sky.xzy, sky, .5 + clamp(pth.y/2., 0., 1.)*.5);

    // Initialize the scene color.
    vec3 sceneCol = sky;



    // The ray has effectively hit the surface, so light it up.
    if(t<FAR){


        // Surface position and surface normal.
        vec3 sp = camPos + t*rd;
        vec3 sn = normal(sp);


        // Ambient occlusion and shadows.
        float ao = calcAO(sp, sn);
        float sh = softShadow(sp + sn*.002, lightPos, 4.); // Set to "1.," if you can do without them.


        // Scene curvature.
        float spr = 4., ampC = 1., offs = .0;
        float crv = curve(sp, spr, ampC, offs);

        // Light direction vector.
        vec3 ld = lightPos - sp;
        // Distance from the light to the surface point.
        float distlpsp = max(length(ld), .001);

        // Normalize the light direction vector.
        ld /= distlpsp;

        // Light attenuation, based on the distances above.
        float atten = MainLightIntensity / (1. + distlpsp*.25); // Apply MainLightIntensity

        // Ambient light.
        float ambience = .5;

        // Diffuse lighting.
        float diff = max( dot(sn, ld), 0.);
        //diff *= diff;

        // Specular lighting.
        //float spec = pow(max( dot( reflect(-ld, sn), -rd), 0.), 32.);
        vec3 hf = (ld - rd)/2.;
        float spec = pow(max(dot(hf, sn), 0.), 8.);

        // Fresnel term. Good for giving a surface a bit of a reflective glow.
        //float fre = pow( clamp(dot(sn, rd) + 1., .0, 1.), 1.);
        //float fre = pow(max(1. - max(dot(-rd, sn), 0.), 0.), 1.); // Fresnel reflection.


        // Object texturing and coloring.
        // Give the rocky surface a blueish charcoal tinge.
        vec3 texCol = mix(vec3(.5, .4, .3), vec3(0, .1, .2),
                          (noise3D(sp*32.)*.66 + noise3D(sp*64.)*.34));
        texCol = mix(texCol, mix(vec3(.16, .1, 0), vec3(.8, .9, .96), (1. - cellTile(sp*4.5)*.75)), RockColorMix); // Use RockColorMix

        // Extra shading..
        texCol *= mix(vec3(.9, .95, 1), vec3(.1, 0, 0),
                      .75 - smoothstep(-.1, .5, cellTile(sp*.5))*.75);

        if(svGID==2){

            // Inner glow structure.
            texCol = mix(texCol, vec3(.03), .5);
            texCol += texCol*svGlow*.1;



        }
        if(svGID==1){

            // Glowing spiral cylinders.
            texCol += texCol*svGlow*.1;

        }

        /*
        // Specular reflection -- Requires the "Forest" cube map.
        float speR = pow(max(dot(normalize(ld - rd), sn), 0.), 8.);
        vec3 rf = reflect(rd, sn); // Surface reflection.
        vec3 rTx = texture(iChannel0, rf).xyz; rTx *= rTx;
        texCol = texCol + texCol*speR*rTx*2.;
        */

        // Using pseudo science to apply a bit of faux back scatter. :)
        float bl = max(dot(-ld, sn), 0.);
        texCol = texCol + texCol*sky*bl*8.;

        //texCol += texCol*sky*fre*fre*4.;// Fresnel.
        texCol += texCol*sky*(sn.y*.5 + .5); // Slight overhead normal lighting.
        //texCol *= 1. + sn.yzx*.25; // Normal color shading.


        // Combining the above terms to produce the final color. It's based more on acheiving a
        // certain aesthetic than science.
        sceneCol = texCol*(diff*sh + ambience + vec3(.2)*spec*sh);


        sceneCol *= crv + .5;
        //sceneCol *= 1. - abs(crv - .5)*2.;

        // Fake reflection. Other that using the refected vector, there's very little science
        // involved, but since the effect is subtle, you can get away with it.
        //vec3 refCol = vec3(1, .05, .075)*smoothstep(.25, 1., noise3D(ref*2.)*.66 + noise3D(ref*4.)*.34 );
        //sceneCol += texCol*refCol*2.;
        // Shading.
        sceneCol *= atten*ao;

        // Apply color palette post-processing
        float current_pulse_value = sin(time * PulseValueAmplitude + length(sp) * 0.1);
        float base_hue_offset_dynamic = fract(time * HueShiftSpeed + current_pulse_value * PulseHueAmplitude);

        vec3 palette_color;

        if (int(PaletteMode) == 0) { // Rainbow Flow
            float hue = fract(base_hue_offset_dynamic + time * PaletteBlendSpeed * 1.5);
            float sat = 0.95 + current_pulse_value * 0.05 * PulseSaturationAmplitude;
            float val = 0.9 + current_pulse_value * 0.1 * PulseValueAmplitude;
            palette_color = hsv2rgb(vec3(hue, sat, val));
        } else if (int(PaletteMode) == 1) { // Complementary Pulse
            float h1 = fract(base_hue_offset_dynamic + time * PaletteBlendSpeed * 0.8);
            float h2 = fract(h1 + 0.5);
            float hue = mix(h1, h2, abs(sin(time * PaletteBlendSpeed * 3.0 + length(sp) * 0.02)));
            float sat = 1.0;
            float val = 0.7 + current_pulse_value * 0.3 * PulseValueAmplitude;
            palette_color = hsv2rgb(vec3(hue, sat, val));
        } else if (int(PaletteMode) == 2) { // Triadic Harmony
            float h_base = fract(base_hue_offset_dynamic + time * PaletteBlendSpeed * 0.6);
            vec3 tri_hues = vec3(h_base, fract(h_base + 1.0/3.0), fract(h_base + 2.0/3.0));
            float hue_index = floor(mod(time * PaletteBlendSpeed * 2.0 + length(sp) * 0.08, 3.0));
            float selected_hue;
            if (hue_index == 0.0) selected_hue = tri_hues.x;
            else if (hue_index == 1.0) selected_hue = tri_hues.y;
            else selected_hue = tri_hues.z;
            float sat = 0.85 + current_pulse_value * 0.15 * PulseSaturationAmplitude;
            float val = 0.75 + current_pulse_value * 0.25 * PulseValueAmplitude;
            palette_color = hsv2rgb(vec3(selected_hue, sat, val));
        } else if (int(PaletteMode) == 3) { // Acid Trip Glitch
            float glitch_factor = fract(sin(time * 15.0 + length(sp) * 0.3) * 200.0);
            float hue = fract(base_hue_offset_dynamic + glitch_factor * PaletteBlendSpeed * 2.5);
            float sat = 1.0;
            float val = 0.5 + current_pulse_value * 0.5 * PulseValueAmplitude;
            palette_color = hsv2rgb(vec3(hue, sat, val));
            palette_color *= (0.6 + sin(time * 35.0) * 0.4);
        } else {
            palette_color = hsv2rgb(vec3(fract(base_hue_offset_dynamic), 0.5, 0.5));
        }

        // Blend the base scene color with the new palette color
        sceneCol = mix(sceneCol, palette_color, 0.5); // You can adjust this mix factor
    }


    //glow = mix(glow, glow.xzy, .35 - rd.y*.35);
    sceneCol += sceneCol*svGlow;


    // Applying fog.
    sceneCol = mix(sky, sceneCol, 1./(t*t/FAR/FAR*5. + 1.));
    //sceneCol = mix(sceneCol, sky, smoothstep(.0, .8, t/FAR));



    // Vignette.
    uv = gl_FragCoord.xy/RENDERSIZE.xy;
    sceneCol = mix(sceneCol, vec3(0),
                    (1. - pow(16.*uv.x*uv.y*(1.-uv.x)*(1. - uv.y), .25))*.5);
    // Clamp and present the pixel to the screen.
    gl_FragColor = vec4(sqrt(max(sceneCol, 0.)), 1);

}