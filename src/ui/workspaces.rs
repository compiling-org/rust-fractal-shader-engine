odular-fractal-shader/src/ui/workspaces.rs</path>
<content lines="1-150">
use egui::{Ui, Vec2, Pos2, Rect};
use std::collections::HashMap;

/// Customizable workspace system for different editing modes
pub struct WorkspaceManager {
    workspaces: HashMap<String, Workspace>,
    current_workspace: String,
    available_workspaces: Vec<String>,
}

impl WorkspaceManager {
    pub fn new() -> Self {
        let mut workspaces = HashMap::new();

        // Create default workspaces
        workspaces.insert("Fractal Modeling".to_string(), Self::create_fractal_modeling_workspace());
        workspaces.insert("Animation".to_string(), Self::create_animation_workspace());
        workspaces.insert("Rendering".to_string(), Self::create_rendering_workspace());
        workspaces.insert("Node Editing".to_string(), Self::create_node_editing_workspace());

        let available_workspaces = vec![
            "Fractal Modeling".to_string(),
            "Animation".to_string(),
            "Rendering".to_string(),
            "Node Editing".to_string(),
        ];

        Self {
            workspaces,
            current_workspace: "Fractal Modeling".to_string(),
            available_workspaces,
        }
    }

    pub fn current_workspace(&self) -> Option<&Workspace> {
        self.workspaces.get(&self.current_workspace)
    }

    pub fn set_workspace(&mut self, name: &str) {
        if self.available_workspaces.contains(&name.to_string()) {
            self.current_workspace = name.to_string();
        }
    }

    pub fn available_workspaces(&self) -> &[String] {
        &self.available_workspaces
    }

    pub fn save_current_layout(&mut self, panels: &[PanelState]) {
        if let Some(workspace) = self.workspaces.get_mut(&self.current_workspace) {
            workspace.panel_states = panels.to_vec();
        }
    }

    fn create_fractal_modeling_workspace() -> Workspace {
        Workspace {
            name: "Fractal Modeling".to_string(),
            description: "Optimized for fractal creation and parameter editing".to_string(),
            panel_states: vec![
                PanelState {
                    panel_type: PanelType::SceneHierarchy,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(250.0, 400.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Left,
                },
                PanelState {
                    panel_type: PanelType::FractalLibrary,
                    position: Pos2::new(0.0, 400.0),
                    size: Vec2::new(250.0, 300.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Left,
                },
                PanelState {
                    panel_type: PanelType::ParameterInspector,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(300.0, 600.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Right,
                },
                PanelState {
                    panel_type: PanelType::Viewport,
                    position: Pos2::new(250.0, 0.0),
                    size: Vec2::new(800.0, 700.0),
                    visible: true,
                    docked: false,
                    dock_side: DockSide::Center,
                },
            ],
        }
    }

    fn create_animation_workspace() -> Workspace {
        Workspace {
            name: "Animation".to_string(),
            description: "Timeline-focused layout for animation work".to_string(),
            panel_states: vec![
                PanelState {
                    panel_type: PanelType::SceneHierarchy,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(200.0, 300.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Left,
                },
                PanelState {
                    panel_type: PanelType::Timeline,
                    position: Pos2::new(0.0, 500.0),
                    size: Vec2::new(1200.0, 200.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Bottom,
                },
                PanelState {
                    panel_type: PanelType::Viewport,
                    position: Pos2::new(200.0, 0.0),
                    size: Vec2::new(1000.0, 500.0),
                    visible: true,
                    docked: false,
                    dock_side: DockSide::Center,
                },
            ],
        }
    }

    fn create_rendering_workspace() -> Workspace {
        Workspace {
            name: "Rendering".to_string(),
            description: "Output and export focused layout".to_string(),
            panel_states: vec![
                PanelState {
                    panel_type: PanelType::RenderSettings,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(300.0, 400.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Right,
                },
                PanelState {
                    panel_type: PanelType::AssetLibrary,
                    position: Pos2::new(0.0, 400.0),
                    size: Vec2::new(300.0, 300.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Right,
                },
                PanelState {
                    panel_type: PanelType::Viewport,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(1000.0, 700.0),
                    visible: true,
                    docked: false,
                    dock_side: DockSide::Center,
                },
            ],
        }
    }

    fn create_node_editing_workspace() -> Workspace {
        Workspace {
            name: "Node Editing".to_string(),
            description: "Full-screen node graph editing".to_string(),
            panel_states: vec![
                PanelState {
                    panel_type: PanelType::NodeLibrary,
                    position: Pos2::new(0.0, 0.0),
                    size: Vec2::new(250.0, 700.0),
                    visible: true,
                    docked: true,
                    dock_side: DockSide::Left,
                },
                PanelState {
                    panel_type: PanelType::NodeGraph,
                    position: Pos2::new(250.0, 0.0),
                    size: Vec2::new(1000.0, 700.0),
                    visible: true,
                    docked: false,
                    dock_side: DockSide::Center,
                },
            ],
        }
    }
}

/// Workspace definition
#[derive(Debug, Clone)]
pub struct Workspace {
    pub name: String,
    pub description: String,
    pub panel_states: Vec<PanelState>,
}

/// Panel state for workspace layouts
#[derive(Debug, Clone)]
pub struct PanelState {
    pub panel_type: PanelType,
    pub position: Pos2,
    pub size: Vec2,
    pub visible: bool,
    pub docked: bool,
    pub dock_side: DockSide,
}

/// Types of panels available
#[derive(Debug, Clone, PartialEq)]
pub enum PanelType {
    SceneHierarchy,
    FractalLibrary,
    ParameterInspector,
    Viewport,
    Timeline,
    RenderSettings,
    AssetLibrary,
    NodeLibrary,
    NodeGraph,
    MaterialEditor,
    LightingControls,
    PerformanceMonitor,
}

/// Dock side for panels
#[derive(Debug, Clone)]
pub enum DockSide {
    Left,
    Right,
    Top,
    Bottom,
    Center,
}

/// Glassmorphism and modern visual effects
pub struct VisualEffects {
    glassmorphism_enabled: bool,
    blur_radius: f32,
    transparency: f32,
    accent_color: egui::Color32,
}

impl VisualEffects {
    pub fn new() -> Self {
        Self {
            glassmorphism_enabled: true,
            blur_radius: 10.0,
            transparency: 0.8,
            accent_color: egui::Color32::from_rgb(0, 255, 255),
        }
    }

    pub fn apply_glassmorphism(&self, ui: &mut Ui, add_contents: impl FnOnce(&mut Ui)) {
        if self.glassmorphism_enabled {
            // Create a semi-transparent background with blur effect
            let rect = ui.available_rect_before_wrap();

            // Draw blurred background (simplified - in real implementation would use GPU blur)
            ui.painter().rect_filled(
                rect,
                8.0,
                egui::Color32::from_rgba_premultiplied(26, 26, 38, (self.transparency * 255.0) as u8),
            );

            // Add subtle border
            ui.painter().rect_stroke(
                rect,
                8.0,
                egui::Stroke::new(1.0, egui::Color32::from_rgb(60, 60, 80)),
            );
        }

        add_contents(ui);
    }

    pub fn accent_button(&self, ui: &mut Ui, text: &str) -> egui::Response {
        let button = egui::Button::new(
            egui::RichText::new(text)
                .color(egui::Color32::WHITE)
                .strong()
        ).fill(self.accent_color.linear_multiply(0.8));

        ui.add(button)
    }
}

/// Pie menu system for context-sensitive actions
pub struct PieMenu {
    center: Pos2,
    radius: f32,
    items: Vec<PieMenuItem>,
    active: bool,
}

impl PieMenu {
    pub fn new(center: Pos2, radius: f32) -> Self {
        Self {
            center,
            radius,
            items: Vec::new(),
            active: false,
        }
    }

    pub fn add_item(&mut self, item: PieMenuItem) {
        self.items.push(item);
    }

    pub fn show(&self, ui: &mut Ui) -> Option<String> {
        if !self.active {
            return None;
        }

        let mut selected_action = None;
        let painter = ui.painter();

        // Draw pie menu background
        painter.circle_filled(
            self.center,
            self.radius,
            egui::Color32::from_rgba_premultiplied(0, 0, 0, 200),
        );

        // Draw pie slices and labels
        let num_items = self.items.len();
        let angle_step = 2.0 * std::f32::consts::PI / num_items as f32;

        for (i, item) in self.items.iter().enumerate() {
            let start_angle = i as f32 * angle_step - std::f32::consts::PI / 2.0;
            let end_angle = (i + 1) as f32 * angle_step - std::f32::consts::PI / 2.0;

            // Draw slice
            let points = self.generate_arc_points(start_angle, end_angle, 32);
            if points.len() >= 3 {
                painter.add(egui::Shape::convex_polygon(
                    points,
                    item.color,
                    egui::Stroke::new(1.0, egui::Color32::WHITE),
                ));
            }

            // Draw icon/label
            let label_pos = self.center + egui::Vec2::angled(start_angle + angle_step / 2.0) * (self.radius * 0.7);
            painter.text(
                label_pos,
                egui::Align2::CENTER_CENTER,
                &item.label,
                egui::FontId::proportional(14.0),
                egui::Color32::WHITE,
            );
        }

        // Check for mouse interaction
        if ui.input(|i| i.pointer.primary_clicked()) {
            if let Some(mouse_pos) = ui.input(|i| i.pointer.interact_pos()) {
                let distance = mouse_pos.distance(self.center);
                if distance <= self.radius {
                    let angle = (mouse_pos - self.center).angle();
                    let normalized_angle = (angle + std::f32::consts::PI / 2.0).rem_euclid(2.0 * std::f32::consts::PI);
                    let item_index = (normalized_angle / angle_step) as usize;
                    if item_index < self.items.len() {
                        selected_action = Some(self.items[item_index].action.clone());
                    }
                }
            }
        }

        selected_action
    }

    pub fn activate(&mut self) {
        self.active = true;
    }

    pub fn deactivate(&mut self) {
        self.active = false;
    }

    fn generate_arc_points(&self, start_angle: f32, end_angle: f32, segments: usize) -> Vec<Pos2> {
        let mut points = vec![self.center];

        for i in 0..=segments {
            let angle = start_angle + (end_angle - start_angle) * (i as f32 / segments as f32);
            let point = self.center + egui::Vec2::angled(angle) * self.radius;
            points.push(point);
        }

        points
    }
}

/// Pie menu item
#[derive(Debug, Clone)]
pub struct PieMenuItem {
    pub label: String,
    pub action: String,
    pub color: egui::Color32,
}

impl PieMenuItem {
    pub fn new(label: String, action: String, color: egui::Color32) -> Self {
        Self {
            label,
            action,
            color,
        }
    }
}