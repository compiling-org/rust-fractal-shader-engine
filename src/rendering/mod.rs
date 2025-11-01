odular-fractal-shader/src/rendering/mod.rs</path>
<content lines="1-200">
pub mod volumetric;
pub mod compositing;
pub mod pbr;

/// Rendering system for fractal visualization
pub struct RenderingEngine {
    volumetric_renderer: VolumetricRenderer,
    compositing_engine: CompositingEngine,
    pbr_renderer: PBRRenderer,
}

impl RenderingEngine {
    pub fn new() -> Self {
        Self {
            volumetric_renderer: VolumetricRenderer::new(),
            compositing_engine: CompositingEngine::new(),
            pbr_renderer: PBRRenderer::new(),
        }
    }

    pub fn render_frame(&mut self, scene: &crate::scene::SceneManager, camera: &crate::scene::Camera) -> RenderResult {
        // Render volumetric effects
        let volumetric_output = self.volumetric_renderer.render(scene, camera);

        // Render PBR surfaces
        let pbr_output = self.pbr_renderer.render(scene, camera);

        // Composite all layers
        let final_output = self.compositing_engine.composite(&[volumetric_output, pbr_output]);

        RenderResult {
            color_buffer: final_output,
            depth_buffer: vec![], // TODO: Implement depth
            performance_stats: PerformanceStats::default(),
        }
    }
}

/// Volumetric rendering for fog, god rays, and density effects
pub struct VolumetricRenderer {
    density_grid: Vec<f32>,
    scattering_coefficients: Vec<f32>,
    absorption_coefficients: Vec<f32>,
}

impl VolumetricRenderer {
    pub fn new() -> Self {
        Self {
            density_grid: Vec::new(),
            scattering_coefficients: Vec::new(),
            absorption_coefficients: Vec::new(),
        }
    }

    pub fn render(&self, scene: &crate::scene::SceneManager, camera: &crate::scene::Camera) -> RenderLayer {
        // Sample volumetric data along camera rays
        let mut volumetric_data = Vec::new();

        // For each pixel in viewport
        for y in 0..camera.viewport_height {
            for x in 0..camera.viewport_width {
                let ray = self.generate_camera_ray(camera, x, y);
                let density = self.sample_density_along_ray(&ray, scene);
                volumetric_data.push(density);
            }
        }

        RenderLayer {
            data: volumetric_data,
            blend_mode: BlendMode::Additive,
            opacity: 0.7,
        }
    }

    fn generate_camera_ray(&self, camera: &crate::scene::Camera, x: u32, y: u32) -> Ray {
        // Convert screen coordinates to world space ray
        let ndc_x = (2.0 * x as f32 / camera.viewport_width as f32) - 1.0;
        let ndc_y = 1.0 - (2.0 * y as f32 / camera.viewport_height as f32);

        // Create ray in camera space
        let ray_direction = nalgebra::Vector3::new(
            ndc_x * (camera.fov * camera.aspect_ratio).tan(),
            ndc_y * camera.fov.tan(),
            -1.0,
        ).normalize();

        // Transform to world space
        let world_direction = camera.view_matrix() * ray_direction;

        Ray {
            origin: camera.position,
            direction: world_direction,
        }
    }

    fn sample_density_along_ray(&self, ray: &Ray, scene: &crate::scene::SceneManager) -> f32 {
        let mut total_density = 0.0;
        let step_size = 0.1;
        let max_distance = 100.0;

        let mut distance = 0.0;
        while distance < max_distance {
            let sample_point = ray.origin + ray.direction * distance;

            // Sample density from all volumetric objects in scene
            for object in scene.objects() {
                if let crate::scene::ObjectType::FractalObject { .. } = &object.object_type {
                    // Sample fractal density
                    let fractal_density = self.sample_fractal_density(&sample_point, object);
                    total_density += fractal_density * step_size;
                }
            }

            distance += step_size;
        }

        total_density
    }

    fn sample_fractal_density(&self, point: &nalgebra::Vector3<f32>, object: &crate::scene::SceneObject) -> f32 {
        // Convert to fractal space
        let local_point = object.transform.inverse() * *point;

        // Sample distance field
        let distance = match &object.object_type {
            crate::scene::ObjectType::FractalObject { formula, .. } => {
                // Use fractal engine to compute distance
                let engine = crate::fractal::engine::FractalEngine::new();
                engine.distance_estimate(local_point).distance
            }
            _ => 0.0,
        };

        // Convert distance to density (negative distance = inside fractal)
        if distance < 0.0 {
            // Density based on distance from surface
            (-distance).min(1.0)
        } else {
            0.0
        }
    }
}

/// Compositing engine for layering multiple render passes
pub struct CompositingEngine {
    layers: Vec<RenderLayer>,
}

impl CompositingEngine {
    pub fn new() -> Self {
        Self {
            layers: Vec::new(),
        }
    }

    pub fn composite(&self, layers: &[RenderLayer]) -> Vec<f32> {
        let mut result = vec![0.0; layers.first().map(|l| l.data.len()).unwrap_or(0)];

        for layer in layers {
            match layer.blend_mode {
                BlendMode::Normal => self.blend_normal(&mut result, &layer.data, layer.opacity),
                BlendMode::Additive => self.blend_additive(&mut result, &layer.data, layer.opacity),
                BlendMode::Multiply => self.blend_multiply(&mut result, &layer.data, layer.opacity),
                BlendMode::Screen => self.blend_screen(&mut result, &layer.data, layer.opacity),
            }
        }

        result
    }

    fn blend_normal(&self, result: &mut [f32], layer: &[f32], opacity: f32) {
        for (i, &value) in layer.iter().enumerate() {
            if i < result.len() {
                result[i] = result[i] * (1.0 - opacity) + value * opacity;
            }
        }
    }

    fn blend_additive(&self, result: &mut [f32], layer: &[f32], opacity: f32) {
        for (i, &value) in layer.iter().enumerate() {
            if i < result.len() {
                result[i] += value * opacity;
            }
        }
    }

    fn blend_multiply(&self, result: &mut [f32], layer: &[f32], opacity: f32) {
        for (i, &value) in layer.iter().enumerate() {
            if i < result.len() {
                result[i] = result[i] * (value * opacity + (1.0 - opacity));
            }
        }
    }

    fn blend_screen(&self, result: &mut [f32], layer: &[f32], opacity: f32) {
        for (i, &value) in layer.iter().enumerate() {
            if i < result.len() {
                result[i] = 1.0 - (1.0 - result[i]) * (1.0 - value * opacity);
            }
        }
    }
}

/// PBR rendering with global illumination
pub struct PBRRenderer {
    irradiance_map: Vec<f32>,
    reflection_map: Vec<f32>,
}

impl PBRRenderer {
    pub fn new() -> Self {
        Self {
            irradiance_map: Vec::new(),
            reflection_map: Vec::new(),
        }
    }

    pub fn render(&self, scene: &crate::scene::SceneManager, camera: &crate::scene::Camera) -> RenderLayer {
        let mut pbr_data = Vec::new();

        // Render all PBR surfaces
        for y in 0..camera.viewport_height {
            for x in 0..camera.viewport_width {
                let color = self.sample_pbr_surface(scene, camera, x, y);
                pbr_data.push(color.x);
                pbr_data.push(color.y);
                pbr_data.push(color.z);
            }
        }

        RenderLayer {
            data: pbr_data,
            blend_mode: BlendMode::Normal,
            opacity: 1.0,
        }
    }

    fn sample_pbr_surface(&self, scene: &crate::scene::SceneManager, camera: &crate::scene::Camera, x: u32, y: u32) -> nalgebra::Vector3<f32> {
        let ray = self.generate_camera_ray(camera, x, y);

        // Find closest intersection with scene geometry
        let mut closest_hit = None;
        let mut min_distance = f32::INFINITY;

        for object in scene.objects() {
            if let Some(hit) = self.ray_object_intersection(&ray, object) {
                if hit.distance < min_distance {
                    min_distance = hit.distance;
                    closest_hit = Some(hit);
                }
            }
        }

        if let Some(hit) = closest_hit {
            // Compute PBR shading
            self.compute_pbr_shading(&hit, scene)
        } else {
            // Background color
            nalgebra::Vector3::new(0.1, 0.1, 0.15)
        }
    }

    fn generate_camera_ray(&self, camera: &crate::scene::Camera, x: u32, y: u32) -> Ray {
        // Similar to volumetric renderer
        let ndc_x = (2.0 * x as f32 / camera.viewport_width as f32) - 1.0;
        let ndc_y = 1.0 - (2.0 * y as f32 / camera.viewport_height as f32);

        let ray_direction = nalgebra::Vector3::new(
            ndc_x * (camera.fov * camera.aspect_ratio).tan(),
            ndc_y * camera.fov.tan(),
            -1.0,
        ).normalize();

        Ray {
            origin: camera.position,
            direction: camera.view_matrix() * ray_direction,
        }
    }

    fn ray_object_intersection(&self, ray: &Ray, object: &crate::scene::SceneObject) -> Option<RayHit> {
        match &object.object_type {
            crate::scene::ObjectType::FractalObject { formula, .. } => {
                // Ray marching for fractal intersection
                self.ray_march_fractal(ray, object, formula)
            }
            crate::scene::ObjectType::MeshObject { .. } => {
                // TODO: Implement mesh intersection
                None
            }
            _ => None,
        }
    }

    fn ray_march_fractal(&self, ray: &Ray, object: &crate::scene::SceneObject, formula: &crate::fractal::types::FractalFormula) -> Option<RayHit> {
        let mut distance = 0.0;
        let max_distance = 100.0;
        let min_distance = 0.001;

        for _ in 0..1000 { // Max steps
            let current_pos = ray.origin + ray.direction * distance;
            let local_pos = object.transform.inverse() * current_pos;

            let engine = crate::fractal::engine::FractalEngine::new();
            let dist = engine.distance_estimate(local_pos).distance;

            if dist < min_distance {
                // Hit!
                let normal = self.compute_normal(local_pos, formula);
                return Some(RayHit {
                    distance,
                    position: current_pos,
                    normal: object.transform.transform_vector(&normal),
                    material_id: 0, // TODO
                });
            }

            distance += dist;

            if distance > max_distance {
                break;
            }
        }

        None
    }

    fn compute_normal(&self, pos: nalgebra::Vector3<f32>, formula: &crate::fractal::types::FractalFormula) -> nalgebra::Vector3<f32> {
        let eps = 0.001;
        let engine = crate::fractal::engine::FractalEngine::new();

        let dx = engine.distance_estimate(pos + nalgebra::Vector3::new(eps, 0.0, 0.0)).distance
               - engine.distance_estimate(pos - nalgebra::Vector3::new(eps, 0.0, 0.0)).distance;
        let dy = engine.distance_estimate(pos + nalgebra::Vector3::new(0.0, eps, 0.0)).distance
               - engine.distance_estimate(pos - nalgebra::Vector3::new(0.0, eps, 0.0)).distance;
        let dz = engine.distance_estimate(pos + nalgebra::Vector3::new(0.0, 0.0, eps)).distance
               - engine.distance_estimate(pos - nalgebra::Vector3::new(0.0, 0.0, eps)).distance;

        nalgebra::Vector3::new(dx, dy, dz).normalize()
    }

    fn compute_pbr_shading(&self, hit: &RayHit, scene: &crate::scene::SceneManager) -> nalgebra::Vector3<f32> {
        let mut color = nalgebra::Vector3::new(0.8, 0.8, 0.8); // Base color

        // Simple diffuse lighting
        for light in &scene.lighting().directional_lights {
            let light_dir = -light.direction;
            let diffuse = hit.normal.dot(&light_dir).max(0.0);
            color = color * (light.color * light.intensity * diffuse + nalgebra::Vector3::new(scene.lighting().ambient_intensity, scene.lighting().ambient_intensity, scene.lighting().ambient_intensity));
        }

        color
    }
}

/// Data structures
pub struct RenderResult {
    pub color_buffer: Vec<f32>,
    pub depth_buffer: Vec<f32>,
    pub performance_stats: PerformanceStats,
}

pub struct RenderLayer {
    pub data: Vec<f32>,
    pub blend_mode: BlendMode,
    pub opacity: f32,
}

#[derive(Clone)]
pub enum BlendMode {
    Normal,
    Additive,
    Multiply,
    Screen,
}

pub struct Ray {
    pub origin: nalgebra::Vector3<f32>,
    pub direction: nalgebra::Vector3<f32>,
}

pub struct RayHit {
    pub distance: f32,
    pub position: nalgebra::Vector3<f32>,
    pub normal: nalgebra::Vector3<f32>,
    pub material_id: u32,
}

pub struct PerformanceStats {
    pub render_time_ms: f32,
    pub rays_cast: u32,
    pub samples_taken: u32,
}

impl Default for PerformanceStats {
    fn default() -> Self {
        Self {
            render_time_ms: 0.0,
            rays_cast: 0,
            samples_taken: 0,
        }
    }
}

impl crate::scene::Camera {
    pub fn viewport_width(&self) -> u32 { 1920 } // TODO: Make configurable
    pub fn viewport_height(&self) -> u32 { 1080 } // TODO: Make configurable
    pub fn aspect_ratio(&self) -> f32 { self.viewport_width() as f32 / self.viewport_height() as f32 }
    pub fn view_matrix(&self) -> nalgebra::Matrix3<f32> {
        // TODO: Implement proper view matrix
        nalgebra::Matrix3::identity()
    }
}

impl crate::scene::Transform {
    pub fn inverse(&self) -> nalgebra::Matrix4<f32> {
        // TODO: Implement proper inverse transform
        nalgebra::Matrix4::identity()
    }

    pub fn transform_vector(&self, v: &nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        // TODO: Implement proper vector transformation
        *v
    }
}