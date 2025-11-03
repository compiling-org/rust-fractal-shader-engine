// use bevy::prelude::*;
use bevy::prelude::*;
use crate::nodes::*;
use std::collections::HashMap;

/// Visual node editor for fractal composition
/// Similar to Blender's node editor interface

// /// Component for visual node representation
// #[derive(Component)]
// pub struct VisualNode {
//     pub node_id: NodeId,
//     pub size: Vec2,
//     pub selected: bool,
// }
//
// /// Component for node input socket
// #[derive(Component)]
// pub struct NodeInputSocket {
//     pub node_id: NodeId,
//     pub input_name: String,
//     pub position: Vec2,
// }
//
// /// Component for node output socket
// #[derive(Component)]
// pub struct NodeOutputSocket {
//     pub node_id: NodeId,
//     pub output_name: String,
//     pub position: Vec2,
// }
//
// /// Component for connection line between nodes
// #[derive(Component)]
// pub struct NodeConnectionLine {
//     pub from_node: NodeId,
//     pub to_node: NodeId,
//     pub from_socket: String,
//     pub to_socket: String,
// }

/// Resource for editor state
#[derive(Resource)]
pub struct NodeEditorState {
    pub selected_nodes: Vec<usize>,
    pub dragged_node: Option<usize>,
    pub drag_offset: Vec2,
    pub connection_start: Option<(usize, String, Vec2)>, // node_id, socket_name, position
    pub pan_offset: Vec2,
    pub zoom: f32,
}

impl Default for NodeEditorState {
    fn default() -> Self {
        Self {
            selected_nodes: Vec::new(),
            dragged_node: None,
            drag_offset: Vec2::ZERO,
            connection_start: None,
            pan_offset: Vec2::ZERO,
            zoom: 1.0,
        }
    }
}

// /// Node editor plugin
// pub struct NodeEditorPlugin;
//
// impl Plugin for NodeEditorPlugin {
//     fn build(&self, app: &mut App) {
//         app
//             .init_resource::<NodeEditorState>()
//             .add_plugins(NodeSystemPlugin)
//             .add_systems(Startup, setup_node_editor)
//             .add_systems(Update, (
//                 handle_node_interactions,
//                 update_visual_nodes,
//                 draw_connection_lines,
//                 handle_editor_input,
//             ));
//     }
// }

// /// Setup the node editor interface
// fn setup_node_editor(
//     mut commands: Commands,
//     mut node_graph: ResMut<NodeGraph>,
// ) {
//     // Create camera for node editor
//     commands.spawn(Camera2d::default());
//
//     // Create sample composition
//     create_sample_composition(&mut node_graph);
//
//     // Create visual representations for nodes
//     spawn_visual_nodes(&mut commands, &node_graph);
// }

// /// Spawn visual representations for all nodes
// fn spawn_visual_nodes(
//     commands: &mut Commands,
//     node_graph: &NodeGraph,
// ) {
//     for (node_id, node) in &node_graph.nodes {
//         spawn_visual_node(commands, node_id, node);
//     }
// }
//
// /// Spawn visual representation for a single node
// fn spawn_visual_node(
//     commands: &mut Commands,
//     node_id: &NodeId,
//     node: &FractalNode,
// ) {
//     let node_width = 200.0;
//     let node_height = 150.0 + (node.inputs.len().max(node.outputs.len()) as f32 * 20.0);
//
//     commands.spawn((
//         Node {
//             position_type: PositionType::Absolute,
//             left: Val::Px(node.position.x),
//             top: Val::Px(node.position.y),
//             width: Val::Px(node_width),
//             height: Val::Px(node_height),
//             ..default()
//         },
//         BackgroundColor(Color::srgb(0.2, 0.2, 0.3)),
//         VisualNode {
//             node_id: *node_id,
//             size: Vec2::new(node_width, node_height),
//             selected: false,
//         },
//     )).with_children(|parent| {
//         // Node title
//         let title = match &node.node_type {
//             NodeType::FractalGenerator(name) => format!("Fractal: {}", name),
//             NodeType::MathOp(op) => format!("Math: {:?}", op),
//             NodeType::ColorAdjust(adj) => format!("Color: {:?}", adj),
//             NodeType::Effect(name) => format!("Effect: {}", name),
//             NodeType::AudioInput => "Audio Input".to_string(),
//             NodeType::MidiInput => "MIDI Input".to_string(),
//             NodeType::Output => "Output".to_string(),
//         };
//
//         parent.spawn((
//             Text::new(title),
//             TextFont {
//                 font_size: 16.0,
//                 ..default()
//             },
//             TextColor(Color::WHITE),
//         ));
//
//         // Input sockets
//         let mut y_offset = 40.0;
//         for (input_name, input) in &node.inputs {
//             parent.spawn((
//                 Node {
//                     position_type: PositionType::Absolute,
//                     left: Val::Px(-8.0),
//                     top: Val::Px(y_offset),
//                     width: Val::Px(16.0),
//                     height: Val::Px(16.0),
//                     ..default()
//                 },
//                 BackgroundColor(Color::srgb(0.8, 0.4, 0.4)),
//                 NodeInputSocket {
//                     node_id: *node_id,
//                     input_name: input_name.clone(),
//                     position: Vec2::new(-8.0, y_offset),
//                 },
//             ));
//
//             parent.spawn((
//                 Text::new(input_name),
//                 TextFont {
//                     font_size: 12.0,
//                     ..default()
//                 },
//                 TextColor(Color::WHITE),
//                 Node {
//                     position_type: PositionType::Absolute,
//                     left: Val::Px(20.0),
//                     top: Val::Px(y_offset),
//                     ..default()
//                 },
//             ));
//
//             y_offset += 25.0;
//         }
//
//         // Output sockets
//         y_offset = 40.0;
//         for (output_name, output) in &node.outputs {
//             parent.spawn((
//                 Node {
//                     position_type: PositionType::Absolute,
//                     right: Val::Px(-8.0),
//                     top: Val::Px(y_offset),
//                     width: Val::Px(16.0),
//                     height: Val::Px(16.0),
//                     ..default()
//                 },
//                 BackgroundColor(Color::srgb(0.4, 0.8, 0.4)),
//                 NodeOutputSocket {
//                     node_id: *node_id,
//                     output_name: output_name.clone(),
//                     position: Vec2::new(node_width - 8.0, y_offset),
//                 },
//             ));
//
//             parent.spawn((
//                 Text::new(output_name),
//                 TextFont {
//                     font_size: 12.0,
//                     ..default()
//                 },
//                 TextColor(Color::WHITE),
//                 Node {
//                     position_type: PositionType::Absolute,
//                     right: Val::Px(25.0),
//                     top: Val::Px(y_offset),
//                     ..default()
//                 },
//             ));
//
//             y_offset += 25.0;
//         }
//     });
// }

// /// Handle node interactions (dragging, selecting, connecting)
// fn handle_node_interactions(
//     mut _commands: Commands,
//     mut node_editor_state: ResMut<NodeEditorState>,
//     mut node_graph: ResMut<NodeGraph>,
//     mouse_button_input: Res<ButtonInput<MouseButton>>,
//     windows: Query<&Window>,
//     mut visual_nodes: Query<(Entity, &mut VisualNode, &mut Node)>,
//     input_sockets: Query<(Entity, &NodeInputSocket)>,
//     output_sockets: Query<(Entity, &NodeOutputSocket)>,
// ) {
//     let window = windows.single().unwrap();
//     let cursor_pos = window.cursor_position().unwrap_or(Vec2::ZERO);
//
//     // Handle mouse interactions
//     if mouse_button_input.just_pressed(MouseButton::Left) {
//         // Check if clicking on a node
//         for (_entity, mut visual_node, mut _style) in visual_nodes.iter_mut() {
//             let node_rect = Rect::from_center_size(visual_node.position(), visual_node.size);
//
//             if node_rect.contains(cursor_pos) {
//                 // Select node
//                 visual_node.selected = true;
//                 node_editor_state.selected_nodes.push(visual_node.node_id);
//                 node_editor_state.dragged_node = Some(visual_node.node_id);
//                 node_editor_state.drag_offset = cursor_pos - visual_node.position();
//
//                 // Change color to indicate selection - using a different approach for Bevy UI
//                 // Note: This might need adjustment based on Bevy version
//                 break;
//             }
//         }
//
//         // Check if clicking on output socket to start connection
//         for (_entity, output_socket) in output_sockets.iter() {
//             let socket_pos = output_socket.position;
//             let socket_rect = Rect::from_center_size(socket_pos, Vec2::new(16.0, 16.0));
//
//             if socket_rect.contains(cursor_pos) {
//                 node_editor_state.connection_start = Some((
//                     output_socket.node_id,
//                     output_socket.output_name.clone(),
//                     socket_pos,
//                 ));
//                 break;
//             }
//         }
//     }
//
//     if mouse_button_input.just_released(MouseButton::Left) {
//         // Finish connection if started
//         if let Some((from_node, from_socket, _start_pos)) = node_editor_state.connection_start.take() {
//             // Check if releasing on input socket
//             for (_entity, input_socket) in input_sockets.iter() {
//                 let socket_pos = input_socket.position;
//                 let socket_rect = Rect::from_center_size(socket_pos, Vec2::new(16.0, 16.0));
//
//                 if socket_rect.contains(cursor_pos) {
//                     // Create connection
//                     let _ = node_graph.connect_nodes(
//                         from_node,
//                         &from_socket,
//                         input_socket.node_id,
//                         &input_socket.input_name,
//                     );
//                     break;
//                 }
//             }
//         }
//
//         // Stop dragging
//         node_editor_state.dragged_node = None;
//     }
//
//     // Handle dragging
//     if let Some(dragged_node_id) = node_editor_state.dragged_node {
//         if let Some(node) = node_graph.nodes.get_mut(&dragged_node_id) {
//             node.position = cursor_pos - node_editor_state.drag_offset;
//         }
//     }
// }

// /// Update visual node positions
// fn update_visual_nodes(
//     node_graph: Res<NodeGraph>,
//     mut visual_nodes: Query<(&mut Node, &VisualNode)>,
// ) {
//     for (mut style, visual_node) in visual_nodes.iter_mut() {
//         if let Some(node) = node_graph.nodes.get(&visual_node.node_id) {
//             style.left = Val::Px(node.position.x);
//             style.top = Val::Px(node.position.y);
//         }
//     }
// }
//
// /// Draw connection lines between nodes
// fn draw_connection_lines(
//     mut _commands: Commands,
//     node_graph: Res<NodeGraph>,
//     _visual_nodes: Query<&VisualNode>,
// ) {
//     // Clear existing connection lines - simplified approach
//     // Note: In a real implementation, you'd need to track connection entities
//     // For now, we'll skip this cleanup to avoid Bevy API issues
//
//     // Draw new connection lines
//     for connection in &node_graph.connections {
//         if let (Some(_from_node), Some(_to_node)) = (
//             node_graph.nodes.get(&connection.from_node),
//             node_graph.nodes.get(&connection.to_node),
//         ) {
//             if let (Some(_from_output), Some(_to_input)) = (
//                 _from_node.outputs.get(&connection.from_output),
//                 _to_node.inputs.get(&connection.to_input),
//             ) {
//                 // Calculate socket positions
//                 let _from_pos = _from_node.position + Vec2::new(200.0, 48.0); // Approximate output socket position
//                 let _to_pos = _to_node.position + Vec2::new(0.0, 48.0); // Approximate input socket position
//
//                 // Create a simple line representation (in a real implementation, you'd use a custom mesh or sprite)
//                 let _mid_point = (_from_pos + _to_pos) / 2.0;
//                 let _distance = _from_pos.distance(_to_pos);
//
//                 // Placeholder for connection line spawning
//                 // In a real implementation, this would spawn visual connection lines
//             }
//         }
//     }
// }
//
// /// Handle editor input (keyboard shortcuts, etc.)
// fn handle_editor_input(
//     mut commands: Commands,
//     keys: Res<ButtonInput<KeyCode>>,
//     mut node_graph: ResMut<NodeGraph>,
//     mut node_editor_state: ResMut<NodeEditorState>,
// ) {
//     // Delete selected nodes
//     if keys.just_pressed(KeyCode::Delete) {
//         for node_id in &node_editor_state.selected_nodes.clone() {
//             node_graph.nodes.remove(node_id);
//             // Remove connections involving this node
//             node_graph.connections.retain(|conn| {
//                 conn.from_node != *node_id && conn.to_node != *node_id
//             });
//         }
//         node_editor_state.selected_nodes.clear();
//     }
//
//     // Add new node (example: press 'F' for fractal generator)
//     if keys.just_pressed(KeyCode::KeyF) {
//         let node_id = node_graph.create_node(
//             NodeType::FractalGenerator("mandelbrot".to_string()),
//             Vec2::new(100.0, 100.0),
//         );
//         spawn_visual_node(&mut commands, &node_id, &node_graph.nodes[&node_id]);
//     }
//
//     // Add math operation node (press 'M')
//     if keys.just_pressed(KeyCode::KeyM) {
//         let node_id = node_graph.create_node(
//             NodeType::MathOp(MathOperation::Add),
//             Vec2::new(150.0, 150.0),
//         );
//         spawn_visual_node(&mut commands, &node_id, &node_graph.nodes[&node_id]);
//     }
//
//     // Add output node (press 'O')
//     if keys.just_pressed(KeyCode::KeyO) {
//         let node_id = node_graph.create_node(
//             NodeType::Output,
//             Vec2::new(300.0, 200.0),
//         );
//         spawn_visual_node(&mut commands, &node_id, &node_graph.nodes[&node_id]);
//     }
// }
//
// /// Helper function to get node position from visual node
// impl VisualNode {
//     pub fn position(&self) -> Vec2 {
//         // This would need to be calculated from the actual transform
//         // For now, return a placeholder
//         Vec2::new(100.0, 100.0)
//     }
// }