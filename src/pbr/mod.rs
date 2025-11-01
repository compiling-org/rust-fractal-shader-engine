odular-fractal-shader/src/pbr/mod.rs</path>
<content lines="1-200">
pub mod pipeline;
pub mod materials;
pub mod lighting;

/// PBR (Physically Based Rendering) pipeline with global illumination
pub struct PBRPipeline {
    irradiance_map: Vec<f32>,
    reflection_map: Vec<f32>,
    brdf_lut: Vec<f32>,
    shadow_maps: Vec<Vec<f32>>,
}

impl PBRPipeline {
    pub fn new() -> Self {
        Self {
            irradiance_map: Vec::new(),
            reflection_map: Vec::new(),
            brdf_lut: Vec::new(),
            shadow_maps: Vec::new(),
        }
    }

    pub fn render(&mut self, scene: &crate::scene::SceneManager, camera: &crate::scene::Camera) -> crate::rendering::RenderLayer {
        // Generate irradiance map for global illumination
        self.generate_irradiance_map(scene);

        // Generate reflection map for specular highlights
        self.generate_reflection_map(scene);

        // Render all PBR surfaces
        let mut pbr_data = Vec::new();

        for y in 0..camera.viewport_height {
            for x in 0..camera.viewport_width {
                let color = self.sample_pbr_surface(scene, camera, x, y);
                pbr_data.push(color.x);
                pbr_data.push(color.y);
                pbr_data.push(color.z);
            }
        }

        crate::rendering::RenderLayer {
            data: pbr_data,
            blend_mode: crate::rendering::BlendMode::Normal,
            opacity: 1.0,
        }
    }

    fn generate_irradiance_map(&mut self, scene: &crate::scene::SceneManager) {
        // Simplified irradiance map generation
        // In a real implementation, this would use spherical harmonics or similar
        self.irradiance_map = vec![0.5; 512 * 512 * 3]; // RGB irradiance

        // Sample lighting from scene
        for light in &scene.lighting().directional_lights {
            // Add directional light contribution
            for i in 0..self.irradiance_map.len() / 3 {
                self.irradiance_map[i * 3] += light.color.x * light.intensity * 0.1;
                self.irradiance_map[i * 3 + 1] += light.color.y * light.intensity * 0.1;
                self.irradiance_map[i * 3 + 2] += light.color.z * light.intensity * 0.1;
            }
        }
    }

    fn generate_reflection_map(&mut self, scene: &crate::scene::SceneManager) {
        // Simplified reflection map generation
        self.reflection_map = vec![0.0; 256 * 256 * 3];

        // Add environment reflections
        for i in 0..self.reflection_map.len() / 3 {
            // Simulate sky reflection
            self.reflection_map[i * 3] = 0.3;     // Blue sky
            self.reflection_map[i * 3 + 1] = 0.5; // Blue sky
            self.reflection_map[i * 3 + 2] = 0.8; // Blue sky
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
            // Background color with atmospheric scattering
            self.sample_skybox(&ray.direction)
        }
    }

    fn generate_camera_ray(&self, camera: &crate::scene::Camera, x: u32, y: u32) -> crate::rendering::Ray {
        let ndc_x = (2.0 * x as f32 / camera.viewport_width as f32) - 1.0;
        let ndc_y = 1.0 - (2.0 * y as f32 / camera.viewport_height as f32);

        let ray_direction = nalgebra::Vector3::new(
            ndc_x * (camera.fov * camera.aspect_ratio).tan(),
            ndc_y * camera.fov.tan(),
            -1.0,
        ).normalize();

        crate::rendering::Ray {
            origin: camera.position,
            direction: camera.view_matrix() * ray_direction,
        }
    }

    fn ray_object_intersection(&self, ray: &crate::rendering::Ray, object: &crate::scene::SceneObject) -> Option<crate::rendering::RayHit> {
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

    fn ray_march_fractal(&self, ray: &crate::rendering::Ray, object: &crate::scene::SceneObject, formula: &crate::fractal::types::FractalFormula) -> Option<crate::rendering::RayHit> {
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
                return Some(crate::rendering::RayHit {
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

    fn compute_pbr_shading(&self, hit: &crate::rendering::RayHit, scene: &crate::scene::SceneManager) -> nalgebra::Vector3<f32> {
        let mut final_color = nalgebra::Vector3::new(0.0, 0.0, 0.0);

        // Base material properties (simplified)
        let albedo = nalgebra::Vector3::new(0.8, 0.8, 0.9); // Light blue-gray
        let metallic = 0.0;
        let roughness = 0.5;
        let ao = 1.0;

        // View direction
        let view_dir = (scene.camera().position - hit.position).normalize();

        // Compute lighting for each light source
        for light in &scene.lighting().directional_lights {
            let light_dir = -light.direction.normalize();
            let half_vector = (view_dir + light_dir).normalize();

            // Cook-Torrance BRDF components
            let ndf = self.normal_distribution_ggx(hit.normal, half_vector, roughness);
            let g = self.geometry_smith(hit.normal, view_dir, light_dir, roughness);
            let f0 = self.fresnel_schlick(hit.normal.dot(&view_dir), nalgebra::Vector3::new(0.04, 0.04, 0.04));
            let f = self.fresnel_schlick(hit.normal.dot(&half_vector), f0);

            let k_s = f;
            let k_d = nalgebra::Vector3::new(1.0, 1.0, 1.0) - k_s;
            k_d = k_d * (1.0 - metallic);

            let numerator = ndf * g * f;
            let denominator = 4.0 * hit.normal.dot(&view_dir) * hit.normal.dot(&light_dir) + 0.0001;
            let specular = numerator / denominator;

            // Add to outgoing radiance
            let n_dot_l = hit.normal.dot(&light_dir).max(0.0);
            final_color += (k_d * albedo / std::f32::consts::PI + specular) * light.color * light.intensity * n_dot_l;
        }

        // Ambient lighting with irradiance
        let irradiance = self.sample_irradiance(&hit.normal);
        let diffuse = irradiance * albedo * ao;
        final_color += diffuse;

        // HDR tonemapping and gamma correction (simplified)
        final_color = final_color / (final_color + nalgebra::Vector3::new(1.0, 1.0, 1.0));
        final_color = nalgebra::Vector3::new(
            final_color.x.powf(1.0 / 2.2),
            final_color.y.powf(1.0 / 2.2),
            final_color.z.powf(1.0 / 2.2),
        );

        final_color
    }

    fn normal_distribution_ggx(&self, n: nalgebra::Vector3<f32>, h: nalgebra::Vector3<f32>, roughness: f32) -> f32 {
        let a = roughness * roughness;
        let a2 = a * a;
        let n_dot_h = n.dot(&h).max(0.0);
        let n_dot_h2 = n_dot_h * n_dot_h;

        let num = a2;
        let denom = n_dot_h2 * (a2 - 1.0) + 1.0;
        let denom = std::f32::consts::PI * denom * denom;

        num / denom
    }

    fn geometry_smith(&self, n: nalgebra::Vector3<f32>, v: nalgebra::Vector3<f32>, l: nalgebra::Vector3<f32>, roughness: f32) -> f32 {
        let ggx2 = self.geometry_schlick_ggx(n.dot(&v), roughness);
        let ggx1 = self.geometry_schlick_ggx(n.dot(&l), roughness);
        ggx1 * ggx2
    }

    fn geometry_schlick_ggx(&self, n_dot_v: f32, roughness: f32) -> f32 {
        let r = roughness + 1.0;
        let k = (r * r) / 8.0;
        n_dot_v / (n_dot_v * (1.0 - k) + k)
    }

    fn fresnel_schlick(&self, cos_theta: f32, f0: nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        f0 + (nalgebra::Vector3::new(1.0, 1.0, 1.0) - f0) * (1.0 - cos_theta).powf(5.0)
    }

    fn sample_irradiance(&self, normal: &nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        // Simplified irradiance sampling
        // In a real implementation, this would sample from the irradiance map
        let u = (normal.x * 0.5 + 0.5).clamp(0.0, 1.0);
        let v = (normal.y * 0.5 + 0.5).clamp(0.0, 1.0);

        let x = (u * 511.0) as usize;
        let y = (v * 511.0) as usize;
        let idx = (y * 512 + x) * 3;

        if idx + 2 < self.irradiance_map.len() {
            nalgebra::Vector3::new(
                self.irradiance_map[idx],
                self.irradiance_map[idx + 1],
                self.irradiance_map[idx + 2],
            )
        } else {
            nalgebra::Vector3::new(0.5, 0.5, 0.5)
        }
    }

    fn sample_skybox(&self, direction: &nalgebra::Vector3<f32>) -> nalgebra::Vector3<f32> {
        // Simple sky gradient
        let t = (direction.y * 0.5 + 0.5).clamp(0.0, 1.0);
        let sky_color = nalgebra::Vector3::new(0.3, 0.6, 1.0); // Blue sky
        let ground_color = nalgebra::Vector3::new(0.2, 0.2, 0.2); // Dark ground

        ground_color.lerp(&sky_color, &nalgebra::Vector3::new(t, t, t))
    }
}