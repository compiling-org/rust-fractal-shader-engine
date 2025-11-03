use bevy::prelude::*;
use std::fs;

/// Simple fractal demo that displays one ISF shader
fn main() {
    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                title: "Rust Fractal Shader Engine - Demo".to_string(),
                resolution: (1280, 720).into(),
                ..default()
            }),
            ..default()
        }))
        .add_systems(Startup, setup)
        .add_systems(Update, update_fractal)
        .run();
}

#[derive(Component)]
struct FractalQuad;

#[derive(Resource)]
struct FractalMaterial {
    material_handle: Handle<StandardMaterial>,
    time: f32,
}

fn setup(
    mut commands: Commands,
    mut meshes: ResMut<Assets<Mesh>>,
    mut materials: ResMut<Assets<StandardMaterial>>,
    asset_server: Res<AssetServer>,
) {
    // Load the "dark fractal" ISF shader
    let isf_path = "assets/shaders/isf/dark fractal.fs";
    let isf_source = match fs::read_to_string(isf_path) {
        Ok(content) => content,
        Err(e) => {
            println!("Failed to load ISF shader: {}", e);
            // Fallback to a simple generated fractal
            r#"
/*{
  "CATEGORIES": ["Generator"],
  "INPUTS": [
    { "NAME": "speed", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.1, "MAX": 5.0 }
  ]
}*/

void main() {
    vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * speed;

    // Simple Mandelbrot-like fractal
    vec2 c = uv * 2.0;
    vec2 z = vec2(0.0);
    float iterations = 0.0;
    const float max_iter = 100.0;

    for(float i = 0.0; i < max_iter; i++) {
        if(length(z) > 2.0) break;
        z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
        iterations = i;
    }

    float fractal = iterations / max_iter;
    vec3 color = vec3(fractal, fractal * 0.5, fractal * 0.8);
    gl_FragColor = vec4(color, 1.0);
}
"#.to_string()
        }
    };

    // Simple fallback shader for now
    let wgsl_source = r#"
@fragment
fn main(@builtin(position) coord: vec4<f32>) -> @location(0) vec4<f32> {
    let uv = coord.xy / vec2<f32>(1280.0, 720.0);
    let t = 0.0; // Would be time in a real shader

    // Simple color gradient
    let r = sin(uv.x * 10.0 + t) * 0.5 + 0.5;
    let g = cos(uv.y * 10.0 + t) * 0.5 + 0.5;
    let b = sin((uv.x + uv.y) * 5.0 + t) * 0.5 + 0.5;

    return vec4<f32>(r, g, b, 1.0);
}
"#.to_string();

    println!("WGSL Shader:\n{}", wgsl_source);

    // Create a simple material (we'll use a basic color for now since custom shaders are complex)
    let material_handle = materials.add(StandardMaterial {
        base_color: Color::srgb(0.5, 0.5, 0.5),
        ..default()
    });

    // Store the material for later use
    commands.insert_resource(FractalMaterial {
        material_handle: material_handle.clone(),
        time: 0.0,
    });

    // Create a quad to display the fractal
    commands.spawn((
        PbrBundle {
            mesh: meshes.add(Mesh::from(bevy::math::primitives::Rectangle::new(2.0, 2.0))),
            material: material_handle,
            transform: Transform::from_xyz(0.0, 0.0, 0.0),
            ..default()
        },
        FractalQuad,
    ));

    // Camera
    commands.spawn(Camera3dBundle {
        transform: Transform::from_xyz(0.0, 0.0, 1.0).looking_at(Vec3::ZERO, Vec3::Y),
        ..default()
    });

    println!("Fractal demo setup complete!");
    println!("ISF Shader loaded from: {}", isf_path);
}

fn update_fractal(
    time: Res<Time>,
    mut fractal_material: ResMut<FractalMaterial>,
    mut materials: ResMut<Assets<StandardMaterial>>,
) {
    // Update the time
    fractal_material.time += time.delta_secs();

    // Update material color based on time (simple animation)
    if let Some(material) = materials.get_mut(&fractal_material.material_handle) {
        let t = fractal_material.time;
        let r = (t.sin() + 1.0) * 0.5;
        let g = ((t * 0.7).sin() + 1.0) * 0.5;
        let b = ((t * 0.5).cos() + 1.0) * 0.5;
        material.base_color = Color::srgb(r, g, b);
    }
}