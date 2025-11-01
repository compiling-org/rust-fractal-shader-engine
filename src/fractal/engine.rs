odular-fractal-shader/src/fractal/engine.rs</path>
<content lines="303-392">
    /// Get current fractal parameters
    pub fn parameters(&self) -> &FractalParameters {
        &self.parameters
    }

    /// Get mutable reference to parameters
    pub fn parameters_mut(&mut self) -> &mut FractalParameters {
        &mut self.parameters
    }

    /// Get quality settings
    pub fn quality_settings(&self) -> &QualitySettings {
        &self.quality_settings
    }

    /// Set quality settings
    pub fn set_quality_settings(&mut self, settings: QualitySettings) {
        self.quality_settings = settings;
    }

    /// Get performance statistics
    pub fn stats(&self) -> &FractalStats {
        &self.stats
    }

    /// Reset performance statistics
    pub fn reset_stats(&mut self) {
        self.stats = FractalStats::default();
    }

    /// Compute color for a distance result
    pub fn compute_color(&self, result: &DistanceResult) -> Vector3<f32> {
        let color_map = &self.parameters.color_map;

        // Base color from iteration count
        let t = (result.iterations as f32 / self.get_max_iterations() as f32).min(1.0);

        // Apply color cycling
        let cycled_t = (t + color_map.cycle_speed * 0.1) % 1.0;

        // Sample from palette
        let palette_index = (cycled_t * (color_map.palette.len() - 1) as f32) as usize;
        let mut color = color_map.palette[palette_index];

        // Apply orbit trap coloring if available
        if let Some(orbit) = result.orbit_trap {
            let orbit_factor = orbit.magnitude() * 0.1;
            color = color + Vector3::new(orbit_factor, orbit_factor * 0.5, orbit_factor * 0.8);
        }

        // Apply saturation and brightness
        color = self.adjust_saturation(color, color_map.saturation);
        color = color * color_map.brightness;

        // Apply contrast
        color = ((color - Vector3::new(0.5, 0.5, 0.5)) * color_map.contrast) + Vector3::new(0.5, 0.5, 0.5);

        // Clamp to valid range
        Vector3::new(
            color.x.clamp(0.0, 1.0),
            color.y.clamp(0.0, 1.0),
            color.z.clamp(0.0, 1.0),
        )
    }

    /// Adjust color saturation
    fn adjust_saturation(&self, color: Vector3<f32>, saturation: f32) -> Vector3<f32> {
        let luminance = color.x * 0.299 + color.y * 0.587 + color.z * 0.114;
        let gray = Vector3::new(luminance, luminance, luminance);
        gray.lerp(&color, &Vector3::new(saturation, saturation, saturation))
    }

    /// Get maximum iterations for current formula
    fn get_max_iterations(&self) -> u32 {
        match &self.parameters.formula {
            FractalFormula::Mandelbrot { max_iterations, .. } => *max_iterations,
            FractalFormula::Mandelbulb { max_iterations, .. } => *max_iterations,
            FractalFormula::Mandelbox { max_iterations, .. } => *max_iterations,
            FractalFormula::IFS { max_iterations, .. } => *max_iterations,
            FractalFormula::QuaternionJulia { max_iterations, .. } => *max_iterations,
            FractalFormula::Custom { .. } => 100, // Default
        }
    }
}

impl Default for FractalEngine {
    fn default() -> Self {
        Self::new()
    }
}