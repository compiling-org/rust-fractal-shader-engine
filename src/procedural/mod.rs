odular-fractal-shader/src/procedural/mod.rs</path>
<content lines="1-200">
pub mod lsystems;
pub mod attractors;
pub mod noise;

/// Procedural motion and generation tools
pub struct ProceduralEngine {
    lsystem_generator: LSystemGenerator,
    attractor_simulator: AttractorSimulator,
    noise_generator: NoiseGenerator,
}

impl ProceduralEngine {
    pub fn new() -> Self {
        Self {
            lsystem_generator: LSystemGenerator::new(),
            attractor_simulator: AttractorSimulator::new(),
            noise_generator: NoiseGenerator::new(),
        }
    }

    pub fn generate_lsystem_path(&self, axiom: &str, rules: &[(char, String)], iterations: u32, angle: f32) -> Vec<nalgebra::Vector3<f32>> {
        self.lsystem_generator.generate_path(axiom, rules, iterations, angle)
    }

    pub fn simulate_attractor(&self, attractor_type: AttractorType, initial_point: nalgebra::Vector3<f32>, iterations: u32) -> Vec<nalgebra::Vector3<f32>> {
        self.attractor_simulator.simulate(attractor_type, initial_point, iterations)
    }

    pub fn generate_noise_field(&self, noise_type: NoiseType, position: nalgebra::Vector3<f32>, scale: f32, octaves: u32) -> f32 {
        self.noise_generator.generate(noise_type, position, scale, octaves)
    }
}

/// L-System generator for procedural growth patterns
pub struct LSystemGenerator {
    turtle_position: nalgebra::Vector3<f32>,
    turtle_direction: nalgebra::Vector3<f32>,
    turtle_angle: f32,
}

impl LSystemGenerator {
    pub fn new() -> Self {
        Self {
            turtle_position: nalgebra::Vector3::zeros(),
            turtle_direction: nalgebra::Vector3::new(0.0, 1.0, 0.0),
            turtle_angle: 0.0,
        }
    }

    pub fn generate_path(&self, axiom: &str, rules: &[(char, String)], iterations: u32, angle: f32) -> Vec<nalgebra::Vector3<f32>> {
        let mut current_string = axiom.to_string();

        // Apply rules for specified iterations
        for _ in 0..iterations {
            let mut new_string = String::new();
            for ch in current_string.chars() {
                if let Some((_, replacement)) = rules.iter().find(|(symbol, _)| *symbol == ch) {
                    new_string.push_str(replacement);
                } else {
                    new_string.push(ch);
                }
            }
            current_string = new_string;
        }

        // Interpret string as turtle graphics
        self.interpret_turtle_commands(&current_string, angle)
    }

    fn interpret_turtle_commands(&self, commands: &str, angle: f32) -> Vec<nalgebra::Vector3<f32>> {
        let mut path = Vec::new();
        let mut position = nalgebra::Vector3::zeros();
        let mut direction = nalgebra::Vector3::new(0.0, 1.0, 0.0);
        let mut stack = Vec::new();

        path.push(position);

        for command in commands.chars() {
            match command {
                'F' | 'G' => {
                    // Move forward and draw
                    position += direction;
                    path.push(position);
                }
                'f' => {
                    // Move forward without drawing
                    position += direction;
                }
                '+' => {
                    // Turn right
                    direction = rotate_vector(direction, angle);
                }
                '-' => {
                    // Turn left
                    direction = rotate_vector(direction, -angle);
                }
                '[' => {
                    // Push state
                    stack.push((position, direction));
                }
                ']' => {
                    // Pop state
                    if let Some((pos, dir)) = stack.pop() {
                        position = pos;
                        direction = dir;
                    }
                }
                _ => {} // Ignore unknown commands
            }
        }

        path
    }
}

/// Strange attractor simulator
pub struct AttractorSimulator;

impl AttractorSimulator {
    pub fn new() -> Self {
        Self
    }

    pub fn simulate(&self, attractor_type: AttractorType, initial_point: nalgebra::Vector3<f32>, iterations: u32) -> Vec<nalgebra::Vector3<f32>> {
        let mut points = Vec::new();
        let mut current = initial_point;

        for _ in 0..iterations {
            points.push(current);
            current = match attractor_type {
                AttractorType::Lorenz => self.lorenz_step(current),
                AttractorType::Rossler => self.rossler_step(current),
                AttractorType::Thomas => self.thomas_step(current),
                AttractorType::Chen => self.chen_step(current),
            };
        }

        points
    }

    fn lorenz_step(&self, point: nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        let sigma = 10.0;
        let rho = 28.0;
        let beta = 8.0 / 3.0;
        let dt = 0.01;

        let dx = sigma * (point.y - point.x);
        let dy = point.x * (rho - point.z) - point.y;
        let dz = point.x * point.y - beta * point.z;

        nalgebra::Vector3::new(
            point.x + dx * dt,
            point.y + dy * dt,
            point.z + dz * dt,
        )
    }

    fn rossler_step(&self, point: nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        let a = 0.2;
        let b = 0.2;
        let c = 5.7;
        let dt = 0.01;

        let dx = -point.y - point.z;
        let dy = point.x + a * point.y;
        let dz = b + point.z * (point.x - c);

        nalgebra::Vector3::new(
            point.x + dx * dt,
            point.y + dy * dt,
            point.z + dz * dt,
        )
    }

    fn thomas_step(&self, point: nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        let b = 0.208186;
        let dt = 0.01;

        let dx = (point.y - point.x) * b;
        let dy = (point.z - point.y) * b;
        let dz = (point.x - point.z) * b;

        nalgebra::Vector3::new(
            point.x + dx * dt,
            point.y + dy * dt,
            point.z + dz * dt,
        )
    }

    fn chen_step(&self, point: nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        let a = 5.0;
        let b = -10.0;
        let c = -0.38;
        let dt = 0.01;

        let dx = a * (point.y - point.x);
        let dy = (c - a) * point.x - point.x * point.z + c * point.y;
        let dz = point.x * point.y - b * point.z;

        nalgebra::Vector3::new(
            point.x + dx * dt,
            point.y + dy * dt,
            point.z + dz * dt,
        )
    }
}

/// Noise generator for procedural textures and motion
pub struct NoiseGenerator;

impl NoiseGenerator {
    pub fn new() -> Self {
        Self
    }

    pub fn generate(&self, noise_type: NoiseType, position: nalgebra::Vector3<f32>, scale: f32, octaves: u32) -> f32 {
        let scaled_pos = position * scale;
        let mut value = 0.0;
        let mut amplitude = 1.0;
        let mut frequency = 1.0;

        for _ in 0..octaves {
            value += match noise_type {
                NoiseType::Perlin => self.perlin_noise(scaled_pos * frequency) * amplitude,
                NoiseType::Simplex => self.simplex_noise(scaled_pos * frequency) * amplitude,
                NoiseType::Value => self.value_noise(scaled_pos * frequency) * amplitude,
                NoiseType::Worley => self.worley_noise(scaled_pos * frequency) * amplitude,
            };

            amplitude *= 0.5;
            frequency *= 2.0;
        }

        value
    }

    fn perlin_noise(&self, pos: nalgebra::Vector3<f32>) -> f32 {
        // Simplified Perlin noise implementation
        let x = pos.x.floor() as i32;
        let y = pos.y.floor() as i32;
        let z = pos.z.floor() as i32;

        let xf = pos.x - x as f32;
        let yf = pos.y - y as f32;
        let zf = pos.z - z as f32;

        let u = fade(xf);
        let v = fade(yf);
        let w = fade(zf);

        let aaa = grad(hash(x, y, z), xf, yf, zf);
        let aba = grad(hash(x, y + 1, z), xf, yf - 1.0, zf);
        let aab = grad(hash(x, y, z + 1), xf, yf, zf - 1.0);
        let abb = grad(hash(x, y + 1, z + 1), xf, yf - 1.0, zf - 1.0);
        let baa = grad(hash(x + 1, y, z), xf - 1.0, yf, zf);
        let bba = grad(hash(x + 1, y + 1, z), xf - 1.0, yf - 1.0, zf);
        let bab = grad(hash(x + 1, y, z + 1), xf - 1.0, yf, zf - 1.0);
        let bbb = grad(hash(x + 1, y + 1, z + 1), xf - 1.0, yf - 1.0, zf - 1.0);

        lerp(w, lerp(v, lerp(u, aaa, baa), lerp(u, aba, bba)),
             lerp(v, lerp(u, aab, bab), lerp(u, abb, bbb)))
    }

    fn simplex_noise(&self, _pos: nalgebra::Vector3<f32>) -> f32 {
        // TODO: Implement Simplex noise
        0.0
    }

    fn value_noise(&self, pos: nalgebra::Vector3<f32>) -> f32 {
        // Simple value noise
        let x = pos.x.floor() as i32;
        let y = pos.y.floor() as i32;
        let z = pos.z.floor() as i32;

        let xf = pos.x - x as f32;
        let yf = pos.y - y as f32;
        let zf = pos.z - z as f32;

        let u = fade(xf);
        let v = fade(yf);
        let w = fade(zf);

        let aaa = random_value(hash(x, y, z));
        let aba = random_value(hash(x, y + 1, z));
        let aab = random_value(hash(x, y, z + 1));
        let abb = random_value(hash(x, y + 1, z + 1));
        let baa = random_value(hash(x + 1, y, z));
        let bba = random_value(hash(x + 1, y + 1, z));
        let bab = random_value(hash(x + 1, y, z + 1));
        let bbb = random_value(hash(x + 1, y + 1, z + 1));

        lerp(w, lerp(v, lerp(u, aaa, baa), lerp(u, aba, bba)),
             lerp(v, lerp(u, aab, bab), lerp(u, abb, bbb)))
    }

    fn worley_noise(&self, pos: nalgebra::Vector3<f32>) -> f32 {
        // Simplified Worley noise (cellular)
        let cell_size = 1.0;
        let cell_x = (pos.x / cell_size).floor() as i32;
        let cell_y = (pos.y / cell_size).floor() as i32;
        let cell_z = (pos.z / cell_size).floor() as i32;

        let mut min_dist = f32::INFINITY;

        for dx in -1..=1 {
            for dy in -1..=1 {
                for dz in -1..=1 {
                    let neighbor_x = cell_x + dx;
                    let neighbor_y = cell_y + dy;
                    let neighbor_z = cell_z + dz;

                    let feature_point = nalgebra::Vector3::new(
                        random_value(hash(neighbor_x, neighbor_y, neighbor_z)) * cell_size + neighbor_x as f32 * cell_size,
                        random_value(hash(neighbor_x, neighbor_y, neighbor_z + 1)) * cell_size + neighbor_y as f32 * cell_size,
                        random_value(hash(neighbor_x, neighbor_y, neighbor_z + 2)) * cell_size + neighbor_z as f32 * cell_size,
                    );

                    let dist = (pos - feature_point).magnitude();
                    min_dist = min_dist.min(dist);
                }
            }
        }

        min_dist
    }
}

/// Helper functions
fn rotate_vector(vec: nalgebra::Vector3<f32>, angle: f32) -> nalgebra::Vector3<f32> {
    let cos_a = angle.cos();
    let sin_a = angle.sin();

    nalgebra::Vector3::new(
        vec.x * cos_a - vec.y * sin_a,
        vec.x * sin_a + vec.y * cos_a,
        vec.z,
    )
}

fn fade(t: f32) -> f32 {
    t * t * t * (t * (t * 6.0 - 15.0) + 10.0)
}

fn lerp(a: f32, b: f32, t: f32) -> f32 {
    a + t * (b - a)
}

fn grad(hash: i32, x: f32, y: f32, z: f32) -> f32 {
    let h = hash & 15;
    let u = if h < 8 { x } else { y };
    let v = if h < 4 { y } else { if h == 12 || h == 14 { x } else { z } };
    (if (h & 1) == 0 { u } else { -u }) + (if (h & 2) == 0 { v } else { -v })
}

fn hash(x: i32, y: i32, z: i32) -> i32 {
    let mut h = x * 92837111 + y * 689287499 + z * 283923481;
    h = ((h << 13) ^ h) & 0x7fffffff;
    (h * h * 60493 + 19990303) & 0x7fffffff
}

fn random_value(seed: i32) -> f32 {
    let mut h = seed;
    h = ((h << 13) ^ h) & 0x7fffffff;
    (h * h * 60493 + 19990303) & 0x7fffffff;
    (h as f32) / 0x7fffffff as f32
}

/// Data types
#[derive(Debug, Clone)]
pub enum AttractorType {
    Lorenz,
    Rossler,
    Thomas,
    Chen,
}

#[derive(Debug, Clone)]
pub enum NoiseType {
    Perlin,
    Simplex,
    Value,
    Worley,
}

impl Default for ProceduralEngine {
    fn default() -> Self {
        Self::new()
    }
}