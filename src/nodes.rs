odular-fractal-shader/src/nodes.rs</path>
<content lines="366-495">
    /// Get input and output sockets for a node type
    fn get_sockets(node_type: &NodeType) -> (HashMap<String, NodeInput>, HashMap<String, NodeOutput>) {
        match node_type {
            NodeType::FractalGenerator(_) => (
                HashMap::from([
                    ("time".to_string(), NodeInput {
                        name: "time".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("zoom".to_string(), NodeInput {
                        name: "zoom".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                ]),
                HashMap::from([
                    ("color".to_string(), NodeOutput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::MathOp(_) => (
                HashMap::from([
                    ("a".to_string(), NodeInput {
                        name: "a".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("b".to_string(), NodeInput {
                        name: "b".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("result".to_string(), NodeOutput {
                        name: "result".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::ColorAdjust(_) => (
                HashMap::from([
                    ("color".to_string(), NodeInput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("color".to_string(), NodeOutput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::Transform => (
                HashMap::from([
                    ("position".to_string(), NodeInput {
                        name: "position".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("rotation".to_string(), NodeInput {
                        name: "rotation".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("scale".to_string(), NodeInput {
                        name: "scale".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                ]),
                HashMap::from([
                    ("transform".to_string(), NodeOutput {
                        name: "transform".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::CustomCode => (
                HashMap::from([
                    ("input1".to_string(), NodeInput {
                        name: "input1".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("output".to_string(), NodeOutput {
                        name: "output".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::PBRMaterial => (
                HashMap::from([
                    ("base_color".to_string(), NodeInput {
                        name: "base_color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.8,
                    }),
                    ("metallic".to_string(), NodeInput {
                        name: "metallic".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("roughness".to_string(), NodeInput {
                        name: "roughness".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.5,
                    }),
                ]),
                HashMap::from([
                    ("material".to_string(), NodeOutput {
                        name: "material".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::ColorGradient => (
                HashMap::from([
                    ("factor".to_string(), NodeInput {
                        name: "factor".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("color".to_string(), NodeOutput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::VolumetricMaterial => (
                HashMap::from([
                    ("density".to_string(), NodeInput {
                        name: "density".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.1,
                    }),
                    ("scattering".to_string(), NodeInput {
                        name: "scattering".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.5,
                    }),
                ]),
                HashMap::from([
                    ("volume".to_string(), NodeOutput {
                        name: "volume".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::TimelineInput => (
                HashMap::new(),
                HashMap::from([
                    ("time".to_string(), NodeOutput {
                        name: "time".to_string(),
                        data_type: DataType::Float,
                    }),
                    ("frame".to_string(), NodeOutput {
                        name: "frame".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::KeyframeInterpolator => (
                HashMap::from([
                    ("time".to_string(), NodeInput {
                        name: "time".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("value".to_string(), NodeOutput {
                        name: "value".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::LogicOp(_) => (
                HashMap::from([
                    ("input1".to_string(), NodeInput {
                        name: "input1".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("input2".to_string(), NodeInput {
                        name: "input2".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("result".to_string(), NodeOutput {
                        name: "result".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::LFO => (
                HashMap::from([
                    ("frequency".to_string(), NodeInput {
                        name: "frequency".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                    ("amplitude".to_string(), NodeInput {
                        name: "amplitude".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                    ("time".to_string(), NodeInput {
                        name: "time".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("output".to_string(), NodeOutput {
                        name: "output".to_string(),
                        data_type: DataType::Float,
                    }),
                ]),
            ),
            NodeType::LightSource => (
                HashMap::from([
                    ("position".to_string(), NodeInput {
                        name: "position".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("color".to_string(), NodeInput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                    ("intensity".to_string(), NodeInput {
                        name: "intensity".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 1.0,
                    }),
                ]),
                HashMap::from([
                    ("light".to_string(), NodeOutput {
                        name: "light".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::Camera => (
                HashMap::from([
                    ("position".to_string(), NodeInput {
                        name: "position".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("target".to_string(), NodeInput {
                        name: "target".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("view_matrix".to_string(), NodeOutput {
                        name: "view_matrix".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::PostProcessing => (
                HashMap::from([
                    ("input".to_string(), NodeInput {
                        name: "input".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("bloom".to_string(), NodeInput {
                        name: "bloom".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                    ("chromatic_aberration".to_string(), NodeInput {
                        name: "chromatic_aberration".to_string(),
                        data_type: DataType::Float,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::from([
                    ("output".to_string(), NodeOutput {
                        name: "output".to_string(),
                        data_type: DataType::Vec3,
                    }),
                ]),
            ),
            NodeType::Output => (
                HashMap::from([
                    ("color".to_string(), NodeInput {
                        name: "color".to_string(),
                        data_type: DataType::Vec3,
                        connected_from: None,
                        default_value: 0.0,
                    }),
                ]),
                HashMap::new(),
            ),
            _ => (HashMap::new(), HashMap::new()),
        }
    }