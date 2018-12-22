for i in geometry as array do
(
	-- Select object
	select i
	
	-- Add VertexPaint modifier
	-- Convert diffuse to vertex color
	modPanel.addModToSelection (VertexPaint ()) ui:on
	
	-- VertexPaint settings
	$.modifiers[#VertexPaint].lightingModel = 2
	$.modifiers[#VertexPaint].useMaps = on 
	
	-- Press button "Assign" in VertexPaint modifier
	SearchButtons = #()
	SearchButtons = windows.getChildrenHWND #max
	
	for j = 1 to SearchButtons.count do
	(
		if SearchButtons[j][5] == "Assign Vertex Colors" do
		(
			SearchButtonPanel = #()
			SearchButtonPanel = windows.getChildrenHWND SearchButtons[j][2]
			
			for j = 1 to SearchButtonPanel.count do
			(
				if SearchButtonPanel[j][5] == "Assign" do
				(
					UIAccessor.PressButton SearchButtonPanel[j][1]
				)
			)
		)
	)
	
	-- Convert to mesh
	macros.run "Modifier Stack" "Convert_to_Mesh"
	
	-- Search all vertex colors
	-- Add unique color to "DiferentColors" array
	numberFaces = getNumFaces i
	DiferentColors = #()
	
	for j = 1 to numberFaces do
	(
		VCFace = getVCFace i j
		VCVertice = getVCFace i j
		VertColor = VCVertice[1] as Integer
		VertColorRGB = getVertColor i VertColor
		appendIfUnique DiferentColors VertColorRGB
	)
	
	-- Create a multi-material with the number of diferent colors
	meditMaterials[1] = Multimaterial ()
	meditMaterials[1] = multiSubMaterial numsubs:DiferentColors.count
	$.material = meditMaterials[1]

	-- Set face material ID and diffuse color based on vertex color
	for j = 1 to numberFaces do
	(
		VCFace = getVCFace i j
		VCVertice = getVCFace i j
		VertColor = VCVertice[1] as Integer
		VertColorRGB = getVertColor i VertColor
		
		for k = 1 to DiferentColors.count do
		(
			if DiferentColors[k] == VertColorRGB do
			(
				setFaceMatID i j k
				meditMaterials[1].materialList[k].name = i.name + "Material" + (k as String)
				meditMaterials[1].materialList[k].Diffuse = DiferentColors[k]
			)
		)
	)
)