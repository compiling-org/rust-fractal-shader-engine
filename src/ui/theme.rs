//! UI Theme System
//!
//! This module provides a comprehensive theme system for the fractal shader studio
//! with professional dark themes, glassmorphism effects, and customizable color schemes.

use egui::{Color32, Vec2, Rounding, Stroke, Style, Visuals, Margin, FontId, FontFamily};
use egui::epaint::{Shadow, Primitive, Rgba};
use std::collections::HashMap;

/// Theme configuration for the fractal shader studio
#[derive(Debug, Clone)]
pub struct Theme {
    pub name: String,
    pub display_name: String,
    pub colors: ThemeColors,
    pub typography: ThemeTypography,
    pub spacing: ThemeSpacing,
    pub components: ComponentStyles,
    pub effects: VisualEffects,
}

/// Color palette for the theme
#[derive(Debug, Clone)]
pub struct ThemeColors {
    // Core theme colors
    pub background: Color32,
    pub surface: Color32,
    pub surface_variant: Color32,
    pub surface_bright: Color32,
    pub surface_dim: Color32,
    
    // Text colors
    pub text_primary: Color32,
    pub text_secondary: Color32,
    pub text_disabled: Color32,
    pub text_inverse: Color32,
    
    // Border and divider colors
    pub border: Color32,
    pub border_light: Color32,
    pub border_focus: Color32,
    
    // Interactive colors
    pub primary: Color32,
    pub primary_variant: Color32,
    pub secondary: Color32,
    pub secondary_variant: Color32,
    
    // Semantic colors
    pub success: Color32,
    pub warning: Color32,
    pub error: Color32,
    pub info: Color32,
    
    // Accent colors
    pub accent_1: Color32,
    pub accent_2: Color32,
    pub accent_3: Color32,
    
    // Node editor specific colors
    pub node_background: Color32,
    pub node_selected: Color32,
    pub node_wire: Color32,
    pub node_wire_hover: Color32,
    pub node_wire_selected: Color32,
    
    // Fractal specific colors
    pub fractal_preview_bg: Color32,
    pub fractal_param_panel: Color32,
    pub fractal_timeline: Color32,
}

/// Typography configuration
#[derive(Debug, Clone)]
pub struct ThemeTypography {
    pub font_family: FontFamily,
    pub base_size: f32,
    pub small_size: f32,
    pub large_size: f32,
    pub title_size: f32,
    pub heading_size: f32,
    pub monospace_size: f32,
    pub line_height: f32,
}

/// Spacing and layout configuration
#[derive(Debug, Clone)]
pub struct ThemeSpacing {
    pub xs: f32,
    pub sm: f32,
    pub md: f32,
    pub lg: f32,
    pub xl: f32,
    pub xxl: f32,
    pub panel_padding: f32,
    pub element_spacing: f32,
    pub section_spacing: f32,
}

/// Component styling
#[derive(Debug, Clone)]
pub struct ComponentStyles {
    pub button: ButtonStyle,
    pub input: InputStyle,
    pub panel: PanelStyle,
    pub card: CardStyle,
    pub node: NodeStyle,
    pub timeline: TimelineStyle,
}

/// Button styling
#[derive(Debug, Clone)]
pub struct ButtonStyle {
    pub background: Color32,
    pub background_hover: Color32,
    pub background_active: Color32,
    pub border: Color32,
    pub text: Color32,
    pub border_radius: f32,
    pub padding: [f32; 4],
}

/// Input field styling
#[derive(Debug, Clone)]
pub struct InputStyle {
    pub background: Color32,
    pub background_focus: Color32,
    pub border: Color32,
    pub border_focus: Color32,
    pub text: Color32,
    pub placeholder: Color32,
    pub border_radius: f32,
}

/// Panel styling
#[derive(Debug, Clone)]
pub struct PanelStyle {
    pub background: Color32,
    pub border: Color32,
    pub border_radius: f32,
    pub shadow: Shadow,
    pub padding: Margin,
}

/// Card styling
#[derive(Debug, Clone)]
pub struct CardStyle {
    pub background: Color32,
    pub border: Color32,
    pub border_radius: f32,
    pub shadow: Shadow,
    pub elevation: f32,
}

/// Node editor styling
#[derive(Debug, Clone)]
pub struct NodeStyle {
    pub background: Color32,
    pub selected: Color32,
    pub hover: Color32,
    pub border: Color32,
    pub border_radius: f32,
    pub header_height: f32,
    pub pin_size: f32,
    pub wire_width: f32,
}

/// Timeline styling
#[derive(Debug, Clone)]
pub struct TimelineStyle {
    pub background: Color32,
    pub track: Color32,
    pub keyframe: Color32,
    pub playhead: Color32,
    pub selection: Color32,
}

/// Visual effects and animations
#[derive(Debug, Clone)]
pub struct VisualEffects {
    pub enable_shadows: bool,
    pub enable_blur: bool,
    pub enable_glow: bool,
    pub glassmorphism: GlassmorphismConfig,
    pub animations: AnimationConfig,
}

/// Glassmorphism configuration
#[derive(Debug, Clone)]
pub struct GlassmorphismConfig {
    pub enabled: bool,
    pub opacity: f32,
    pub blur_strength: f32,
    pub tint_color: Color32,
    pub border_opacity: f32,
}

/// Animation configuration
#[derive(Debug, Clone)]
pub struct AnimationConfig {
    pub transition_duration: f32,
    pub easing_function: String,
    pub hover_transitions: bool,
    pub button_press_duration: f32,
}

/// Predefined professional themes
pub struct ProfessionalThemes;

impl ProfessionalThemes {
    /// Deep space theme - dark with blue accents
    pub fn deep_space() -> Theme {
        Theme {
            name: "deep_space".to_string(),
            display_name: "Deep Space".to_string(),
            colors: ThemeColors {
                background: Color32::from_rgb(12, 14, 20),
                surface: Color32::from_rgb(18, 22, 30),
                surface_variant: Color32::from_rgb(25, 30, 40),
                surface_bright: Color32::from_rgb(35, 42, 55),
                surface_dim: Color32::from_rgb(10, 12, 18),
                
                text_primary: Color32::from_rgb(240, 240, 245),
                text_secondary: Color32::from_rgb(180, 185, 195),
                text_disabled: Color32::from_rgb(100, 105, 115),
                text_inverse: Color32::from_rgb(15, 18, 25),
                
                border: Color32::from_rgb(45, 52, 65),
                border_light: Color32::from_rgb(35, 42, 55),
                border_focus: Color32::from_rgb(100, 140, 200),
                
                primary: Color32::from_rgb(100, 140, 200),
                primary_variant: Color32::from_rgb(80, 120, 180),
                secondary: Color32::from_rgb(150, 100, 180),
                secondary_variant: Color32::from_rgb(120, 80, 150),
                
                success: Color32::from_rgb(100, 180, 120),
                warning: Color32::from_rgb(200, 160, 80),
                error: Color32::from_rgb(200, 100, 100),
                info: Color32::from_rgb(100, 160, 200),
                
                accent_1: Color32::from_rgb(80, 200, 220),
                accent_2: Color32::from_rgb(200, 120, 180),
                accent_3: Color32::from_rgb(180, 160, 100),
                
                node_background: Color32::from_rgb(30, 35, 45),
                node_selected: Color32::from_rgb(80, 120, 180),
                node_wire: Color32::from_rgb(120, 130, 150),
                node_wire_hover: Color32::from_rgb(150, 160, 180),
                node_wire_selected: Color32::from_rgb(100, 140, 200),
                
                fractal_preview_bg: Color32::from_rgb(8, 10, 15),
                fractal_param_panel: Color32::from_rgb(25, 28, 35),
                fractal_timeline: Color32::from_rgb(20, 25, 30),
            },
            typography: ThemeTypography {
                font_family: FontFamily::Proportional,
                base_size: 14.0,
                small_size: 11.0,
                large_size: 16.0,
                title_size: 18.0,
                heading_size: 16.0,
                monospace_size: 12.0,
                line_height: 1.4,
            },
            spacing: ThemeSpacing {
                xs: 4.0,
                sm: 8.0,
                md: 12.0,
                lg: 16.0,
                xl: 24.0,
                xxl: 32.0,
                panel_padding: 12.0,
                element_spacing: 8.0,
                section_spacing: 16.0,
            },
            components: ComponentStyles {
                button: ButtonStyle {
                    background: Color32::from_rgb(30, 35, 45),
                    background_hover: Color32::from_rgb(40, 45, 55),
                    background_active: Color32::from_rgb(25, 30, 40),
                    border: Color32::from_rgb(60, 70, 85),
                    text: Color32::from_rgb(220, 225, 235),
                    border_radius: 6.0,
                    padding: [8.0, 12.0, 8.0, 12.0],
                },
                input: InputStyle {
                    background: Color32::from_rgb(25, 30, 35),
                    background_focus: Color32::from_rgb(30, 35, 40),
                    border: Color32::from_rgb(50, 60, 75),
                    border_focus: Color32::from_rgb(100, 140, 200),
                    text: Color32::from_rgb(220, 225, 235),
                    placeholder: Color32::from_rgb(120, 130, 145),
                    border_radius: 4.0,
                },
                panel: PanelStyle {
                    background: Color32::from_rgb(18, 22, 28),
                    border: Color32::from_rgb(35, 42, 55),
                    border_radius: 8.0,
                    shadow: Shadow {
                        offset: [0, 2],
                        blur: 8,
                        spread: 0,
                        color: Color32::from_black_alpha(80),
                    },
                    padding: Margin::same(12),
                },
                card: CardStyle {
                    background: Color32::from_rgb(22, 26, 32),
                    border: Color32::from_rgb(40, 48, 60),
                    border_radius: 10.0,
                    shadow: Shadow {
                        offset: [0, 4],
                        blur: 16,
                        spread: 0,
                        color: Color32::from_black_alpha(80),
                    },
                    elevation: 4.0,
                },
                node: NodeStyle {
                    background: Color32::from_rgb(30, 35, 45),
                    selected: Color32::from_rgb(80, 120, 180),
                    hover: Color32::from_rgb(40, 45, 60),
                    border: Color32::from_rgb(60, 70, 85),
                    border_radius: 8.0,
                    header_height: 24.0,
                    pin_size: 8.0,
                    wire_width: 2.0,
                },
                timeline: TimelineStyle {
                    background: Color32::from_rgb(15, 18, 25),
                    track: Color32::from_rgb(25, 30, 35),
                    keyframe: Color32::from_rgb(100, 140, 200),
                    playhead: Color32::from_rgb(200, 160, 80),
                    selection: Color32::from_rgb(80, 120, 180),
                },
            },
            effects: VisualEffects {
                enable_shadows: true,
                enable_blur: true,
                enable_glow: false,
                glassmorphism: GlassmorphismConfig {
                    enabled: true,
                    opacity: 0.15,
                    blur_strength: 12.0,
                    tint_color: Color32::from_rgb(255, 255, 255),
                    border_opacity: 0.2,
                },
                animations: AnimationConfig {
                    transition_duration: 0.15,
                    easing_function: "ease-in-out".to_string(),
                    hover_transitions: true,
                    button_press_duration: 0.08,
                },
            },
        }
    }

    /// Neon synthwave theme - dark with vibrant neon accents
    pub fn neon_synthwave() -> Theme {
        let mut theme = Self::deep_space();
        theme.name = "neon_synthwave".to_string();
        theme.display_name = "Neon Synthwave".to_string();
        
        // Override with neon colors
        theme.colors.primary = Color32::from_rgb(255, 64, 129); // Neon pink
        theme.colors.secondary = Color32::from_rgb(0, 229, 255); // Neon cyan
        theme.colors.accent_1 = Color32::from_rgb(255, 234, 0); // Neon yellow
        theme.colors.accent_2 = Color32::from_rgb(156, 39, 176); // Purple
        theme.colors.accent_3 = Color32::from_rgb(76, 175, 80); // Green
        theme.colors.node_wire = Color32::from_rgb(0, 229, 255);
        theme.colors.node_wire_selected = Color32::from_rgb(255, 64, 129);
        
        theme.effects.enable_glow = true;
        theme.effects.glassmorphism.opacity = 0.2;
        theme.effects.glassmorphism.blur_strength = 16.0;
        
        theme
    }

    /// Midnight blue theme - professional with blue undertones
    pub fn midnight_blue() -> Theme {
        let mut theme = Self::deep_space();
        theme.name = "midnight_blue".to_string();
        theme.display_name = "Midnight Blue".to_string();
        
        theme.colors.background = Color32::from_rgb(16, 20, 32);
        theme.colors.surface = Color32::from_rgb(24, 28, 40);
        theme.colors.primary = Color32::from_rgb(64, 120, 200);
        theme.colors.secondary = Color32::from_rgb(120, 80, 180);
        theme.colors.border = Color32::from_rgb(40, 48, 64);
        theme.colors.node_background = Color32::from_rgb(32, 36, 48);
        
        theme
    }

    /// Forest green theme - professional with green accents
    pub fn forest_green() -> Theme {
        let mut theme = Self::deep_space();
        theme.name = "forest_green".to_string();
        theme.display_name = "Forest Green".to_string();
        
        theme.colors.background = Color32::from_rgb(14, 18, 16);
        theme.colors.surface = Color32::from_rgb(20, 26, 22);
        theme.colors.primary = Color32::from_rgb(76, 175, 80);
        theme.colors.secondary = Color32::from_rgb(139, 195, 74);
        theme.colors.border = Color32::from_rgb(46, 58, 48);
        theme.colors.node_background = Color32::from_rgb(28, 32, 30);
        theme.colors.fractal_preview_bg = Color32::from_rgb(10, 12, 10);
        
        theme
    }
}

/// Theme manager for switching and customizing themes
pub struct ThemeManager {
    themes: HashMap<String, Theme>,
    current_theme: String,
    custom_colors: HashMap<String, Color32>,
}

impl ThemeManager {
    pub fn new() -> Self {
        let mut manager = Self {
            themes: HashMap::new(),
            current_theme: "deep_space".to_string(),
            custom_colors: HashMap::new(),
        };
        
        // Load all professional themes
        manager.add_theme(ProfessionalThemes::deep_space());
        manager.add_theme(ProfessionalThemes::neon_synthwave());
        manager.add_theme(ProfessionalThemes::midnight_blue());
        manager.add_theme(ProfessionalThemes::forest_green());
        
        manager
    }

    /// Add a theme to the manager
    pub fn add_theme(&mut self, theme: Theme) {
        self.themes.insert(theme.name.clone(), theme);
    }

    /// Get current theme
    pub fn current_theme(&self) -> Option<&Theme> {
        self.themes.get(&self.current_theme)
    }

    /// Get mutable reference to current theme
    pub fn current_theme_mut(&mut self) -> Option<&mut Theme> {
        self.themes.get_mut(&self.current_theme)
    }

    /// Switch to a theme by name
    pub fn set_theme(&mut self, theme_name: &str) -> bool {
        if self.themes.contains_key(theme_name) {
            self.current_theme = theme_name.to_string();
            true
        } else {
            false
        }
    }

    /// Get all available theme names
    pub fn available_themes(&self) -> Vec<String> {
        self.themes.keys().cloned().collect()
    }

    /// Apply theme to egui context
    pub fn apply_to_egui(&self, ctx: &egui::Context) {
        if let Some(theme) = self.current_theme() {
            self.apply_theme_visuals(ctx, theme);
        }
    }

    /// Apply theme visuals to egui
    fn apply_theme_visuals(&self, ctx: &egui::Context, theme: &Theme) {
        let mut style = (*ctx.style()).clone();
        
        // Apply colors
        style.visuals = self.create_egui_visuals(theme);
        
        // Apply typography
        self.apply_typography(&mut style, theme);
        
        // Apply spacing
        self.apply_spacing(&mut style, theme);
        
        ctx.set_style(style);
    }

    /// Create egui visuals from theme
    fn create_egui_visuals(&self, theme: &Theme) -> Visuals {
        let mut visuals = Visuals::dark();
        
        // Background colors
        visuals.panel_fill = theme.colors.background;
        visuals.window_fill = theme.colors.surface;
        visuals.faint_bg_color = theme.colors.surface_dim;
        visuals.extreme_bg_color = theme.colors.background;
        
        // Text colors
        visuals.override_text_color = Some(theme.colors.text_primary);
        visuals.hyperlink_color = theme.colors.primary;
        visuals.warn_fg_color = theme.colors.warning;
        visuals.error_fg_color = theme.colors.error;
        
        // Widget styling
        visuals.widgets.inactive.bg_fill = theme.colors.surface_variant;
        visuals.widgets.inactive.fg_stroke = Stroke::new(1.0, theme.colors.text_secondary);
        
        visuals.widgets.hovered.bg_fill = theme.colors.surface_bright;
        visuals.widgets.hovered.fg_stroke = Stroke::new(1.0, theme.colors.text_primary);
        
        visuals.widgets.active.bg_fill = theme.colors.primary;
        visuals.widgets.active.fg_stroke = Stroke::new(1.0, theme.colors.text_inverse);
        
        // Selection
        visuals.selection.bg_fill = theme.colors.primary;
        visuals.selection.stroke = Stroke::new(2.0, theme.colors.primary_variant);
        
        // Window styling
        visuals.window_shadow = if theme.effects.enable_shadows {
            Shadow {
                offset: [0i8, 8i8],
                blur: 24u8,
                spread: 0u8,
                color: Color32::from_black_alpha(128),
            }

        } else {
            Shadow::NONE
        };
        
        visuals
    }

    /// Apply typography settings
    fn apply_typography(&self, style: &mut Style, theme: &Theme) {
        let typography = &theme.typography;
        
        // Set text styles
        style.text_styles.insert(
            egui::TextStyle::Small,
            FontId::new(typography.small_size, typography.font_family.clone()),
        );

        style.text_styles.insert(
            egui::TextStyle::Body,
            FontId::new(typography.base_size, typography.font_family.clone()),
        );

        style.text_styles.insert(
            egui::TextStyle::Heading,
            FontId::new(typography.heading_size, typography.font_family.clone()),
        );

        style.text_styles.insert(
            egui::TextStyle::Name("Heading".into()),
            FontId::new(typography.title_size, typography.font_family.clone()),
        );

        style.text_styles.insert(
            egui::TextStyle::Monospace,
            FontId::new(typography.monospace_size, FontFamily::Monospace),
        );
    }

    /// Apply spacing settings
    fn apply_spacing(&self, style: &mut Style, theme: &Theme) {
        let spacing = &theme.spacing;
        
        style.spacing.window_margin = Margin::same(spacing.md as i8);
        style.spacing.item_spacing = Vec2::new(spacing.element_spacing, spacing.element_spacing);
        style.spacing.button_padding = Vec2::new(spacing.sm, spacing.sm);
    }

    /// Create glassmorphism effect
    pub fn create_glassmorphism_paint(&self, theme: &Theme) -> egui::PaintCallback {
        if !theme.effects.glassmorphism.enabled {
            return egui::PaintCallback {
                rect: egui::Rect::ZERO,
                callback: std::sync::Arc::new(|_info: &egui::PaintCallbackInfo, _add_contents: &mut egui::Painter, _user_data: &egui::epaint::PaintCallback| {
                    // Glassmorphism implementation would go here
                }),
            };
        }

        let config = &theme.effects.glassmorphism;

        egui::PaintCallback {
            rect: egui::Rect::ZERO,
            callback: std::sync::Arc::new(|_info: &egui::PaintCallbackInfo, _add_contents: &mut egui::Painter, _user_data: &egui::epaint::PaintCallback| {
                // Glassmorphism implementation would go here
            }),
        }
    }

    /// Get theme color with fallback
    pub fn get_color(&self, theme_name: &str, color_key: &str) -> Option<Color32> {
        if let Some(theme) = self.themes.get(theme_name) {
            // This would implement color lookup by key
            // For now, return None
            None
        } else {
            None
        }
    }

    /// Create custom theme variant
    pub fn create_custom_theme(&mut self, base_theme: &str, modifications: &[(&str, Color32)]) -> String {
        if let Some(base) = self.themes.get(base_theme).cloned() {
            let mut custom = base;
            let custom_name = format!("custom_{}", self.themes.len());
            custom.name = custom_name.clone();
            custom.display_name = format!("Custom {}", custom.display_name);
            
            // Apply modifications
            for (key, color) in modifications {
                // This would apply color modifications based on key
                match *key {
                    "primary" => custom.colors.primary = *color,
                    "background" => custom.colors.background = *color,
                    _ => {}
                }
            }
            
            self.add_theme(custom);
            custom_name
        } else {
            "deep_space".to_string()
        }
    }
}

impl Default for ThemeManager {
    fn default() -> Self {
        Self::new()
    }
}