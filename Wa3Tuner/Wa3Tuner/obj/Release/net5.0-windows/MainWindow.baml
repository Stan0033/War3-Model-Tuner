   M S B A M L   `   `   `  ����   Wa3Tuner    Wa3Tuner.MainWindow   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -   $��
App_window��
�������  � WindowStartupLocation$  CenterScreen=� � Title$ War3 Model Tuner��$	��500��$	��550��.���  � ColumnDefinitions �� ��  � RowDefinitions �� $��20 ��� .+�o� $��0��
�������$��2��$��20��.@�m� $��Model��.@�m� -   $
��Load��m� -   $��Reload��m� -   $��
ButtonSave��$
��Save��${�False��m� -   $��ButtonSaveAs��$��
Save as...��${�False��m� -   $��ButtonOpenModelFolder��$��Open containing folder��m� -   $��Set model name��m� -   $��Item_Recents��$��Recents��m� $��UI��.@�m� -	   $��Refresh all lists��m� -
   $��Make a screenshot��m� -   $$��Make a screenshot of the model��m� $��Render��.@�m� -    m� IsCheckable$
 True�� m� 	IsChecked$
 True��$��Textured��m� 
�������$��Import��.@�m� -   $��Import all geosets of...��m� -   $��Import all nodes of..��m� -   $��Import all textures of...��m� $��Cleaner��.@�m� -   $��Remove all lights��m� -   $��Remove all emitter 1s��m� -   $��Remove all emitter 2s��m� -   $��Remove all attachments��m� -   $!��Remove all collision shapes��m� -   $��Remove all helpers��m� -   $��Remove all event objects��m� -   $��Remove all nodes��m� -   $��Remove all geosets��m� -   $-��'Reset all geoset animations to static 1��m� -   $#��Remove all texture animations��m� -   $��Remove all cameras��m� -   $��Remove all sequences��m� -   $!��Remove all global sequences��m� -   $��Remove all geosets��m� -   $ ��Remove all node animations��m� -    $��	UV Mapper��m� $
��Help��.@�m� -!   $��How a model works?��m� -"   $��About��'� -#   $��Viewport_Main��$��1��$��0��$y�	Collapsed=��� -$   $��ListOptions��$��1��$��0��$��2��.@��� $��Optimize��$��2��$��2��
�������.���  �� $	��93* ��� $
��451* � �� $��70 ��� �� $��70 �.+��� -%   $��Stack_Optimize��$	��700��
�������$��2��$��
0,0,0,-264q�$��3��.+��� -&   .��	Check all�� -'   .��Uncheck all�� -(   .��Inverse Check�� $��1��$��2��.���� -)   $��ListOptOptions��
�������.+���  d� 	IsChecked$
 TrueY�${�False��.��#!Re-arrange inconsistent sequences�� $
 TrueY�${�False��.��#!Re-arrange inconsistent keyframes�� $
 TrueY�${�False��.��add missing pivot points�� $
 TrueY�${�False��.�� Fix invalid node relationships�� $
 TrueY�${�False��.��Remove empty geosets�� $
 TrueY�${�False��.��$"Create layer for materials without�� -*   $��Check_Linearize��.��Linearize animations�� -+   $��Check_delEmptyEO��.��Delete empty event objects�� -,   $ ��check_delISolatedTriangles��.��Delete isolated triangles�� --   $��check_delISolatedVertices��.��Delete isolated vertices�� -.   $"��Check_Delete0LengthSequences��.��Delete 0-length sequences�� -/   $#��Check_Delete0LengthGSequences��.��" Delete 0-length global sequences�� -0   $%��Check_DeleteUnanimatedSequences��.��Delete un-animated sequences�� -1   $��Check_delunusedgs��.��Delete unused global sequencs�� -2   $��Check_ConvertBones��.��0.Remove bones with no attachees and no children�� -3   $��Check_DeleteUnusedHelpers��.��Delete unused helpers�� -4   $!��Check_DeleteUnusedMaterials��.��Delete unused materials�� -5   $ ��Check_DeleteUnusedTextures��.��Delete unused textures�� -6   $)��#Check_DeleteUnusedTextureAnimations��.��" Delete unused texture animations�� -7   $!��Check_DeleteUnusedKeyframes��.��Delete unused keyframes�� -8   $-��'Check_DeleteIdenticalAdjascentKEyframes��.��&$Delete identical adjascent keyframes�� -9   $3��-Check_DeleteIdenticalAdjascentKEyframes_times��.��.,Delete identical adjascent keyframes's times�� -:   $)��#Check_DeleteSimilarSimilarKEyframes��${�False��.��$"Delete similar adjascent keyframes�� -;   $��Check_MissingKeyframes��.��Add missing opening keyframes�� -<   $��Check_MergeGeosets��.��+)Merge geosets that use the same materials�� -=   $$��Check_DeleteUnusedMatrixGroups��.��Delete unused matrix groups�� ->   $��Check_CalculateExtents��.��Recalculate extents�� -?   $��Check_ClampKeyframes��.��clamp all keyframes�� -@   $��Check_ClampUVs��.��clamp all uv coordinates�� -A   $��Check_ClampLights��.��clamp all light attentuations�� -B   $��Check_AddMissingOrigin��.��add origin ref if missing�� -C   $��Check_DeleteDuplciateGAs��.��$"delete duplicate geoset animations�� -D   $!��Check_AddMissingGeosetAnims��.��add missing geoset animations�� -E   $��Check_SetAllStaticGAs��.��'%set all static geoset animations to 1�� -F   $"��Check_AddMissingVisibilities��.��=;add missing visibilities to all geoset animations and nodes�� -G   $��Check_DetachFromNonBone��.��IGdetach vertices from non-bone nodes and if empy, reattach to dummy bone�� -H   $��Check_VisInterp��.��+)set interpolation of visibilities to none�� -I   $��1��$��2��$��	0,296,0,0q�$��2��.��
Optimize�� $��Geosets��
�������.��� 
������� �� $��* ��� $	��200��$	��450�� �� $��20 ��� $��* ��� $��25 ��� $��* �.+�'� -J   $��Scene_Viewport��$��0��$��4���� -K   $��SceneOVerlay��$��0��$��4��
������ ��  �� Orientation$ Vertical=�$��1��$��0���� -L   $��Report_Geosets��$��Geosets��$
��Bold"�$��1��$��0���� -M   $��ButtonGeosetOptions��$	��...��$��30��$��1��$
��Bold"�$��0��$	��Top=�
������� � ContextMenu �� .@�m� -N   $��
Select All��m� -O   $��Select None��m� -P   $��Invert Select��m� -Q   $��Edit visibilities��m� -R   $��Change used material��m� -S   $��Delete��m� $��Geoset Operations��.@�m� $��	Center...��.@�m� -T   $��	Center...��m� -U   $��Center at...��m� $��
Flatten...��.@�m� -V   $��
Flatten...��m� -W   $��Flatten side...��m� -X   $��Put on ground...��m� -Y   $��Translate...��m� -Z   $��	Rotate...��m� -[   $��Scale each...��m� -\   $��	Negate...��m� -]   $��Average normals��m� -^   $��Merge��	 � ToolTip$O	 IAll selected geosets will be merged into one and use the first's material��m� -_   $��Pull together...��m� -`   $��	Subdivide��$K	 ESplits each triangle into 3 triangles. Preserves texture coordinates.��m� -a   $��Simplify��$u	 oFor each flat surface found inside the selected geoset, reduces it to the minimum required triangles to form it��m� -b   $��	Duplicate��m� -c   $��Align...��m� -d   $��Scale to fit...��m� -e   $,��&Create collision shape for this geoset��m� -f   $��Fragment��$S	 MSplits a geoset into multiple geosets, each holding a single of its triangles��m� -g   $��Weld all��$>	 8Merges all vertices that overlap of each selected geoset��m� -h   $%��Reattach all to another bone...��m� $��Create��.@�m� -i   $&�� Create Hero team glow (and bone)��m� -j   $'��!Create Team background (and bone)��m� $��Create Primitive shape...��.@�m� -k   $
��Cube��m� -l   $��Cylinder��m� -m   $
��Cone��m� $��UV��.@�m� -n   $��	Clamp All��m� -o   $��	Negate Us��m� -p   $��	Negate Vs��m� -q   $��Swap Us and Vs��m� -r   $	��Fit��$\	 VIf the geoset has 2 triangles, their textue coordintes will be fitted onto the texture���� -s   $��ListGeosets��$��1��$��1��
 �� SelectionMode$
 Multiple=��� -t   $��List_GeosetAnims��$��3��$��1��$
 Single=��� $ 
Horizontal=�$��2��$��1��
�������.+��� -u   $��	Label_GAs��$��Geoset animations��$
��Bold"�$��0,0,5,0q��� -v   $��	Buttongam��$	��...��$��30��$��1��$
��Bold"�$��2��$	��Top=�
������� �� .@�m� -w   $��view transformations��m� -x   $��Make use/not use color��m� -y   $-��'Make all geoset animations alpha static��m� -z   $@��:equalize the alpha animation data of all geoset animations��m� -{   $@��:equalize the color animation data of all geoset animations��m� -|   $��Edit alphas��m� -}   $��Edit colors���� $��	Sequences��
�������.��� 
������� �� $
��203* ��� $	��69* ��� $
��272* � �� $��20 ��� �� .+��� -~   $��Report_sequences��$��0��$��0��$��2���� -   $��Report_gsequences��$��0��$��2���� -�   $��ListSequenes��
�������$
 Single=�$��1��$��2���� -�   $��ListGSequenes��
�������$��1��$��2��$
 Single=��� $��Center=�$��0��$��2��$��2��.���� 
�������$
��1000��.+��� -�   .��New�� -�   .��Rename�� -�   .��Swap names with...�� -�   .��%#Duplicate (with matching keyframes)�� -�   .��" Resize... (along with keyframes)�� -�   .��#!Delete (along with all keyframes)�� -�   .��Make looping/Nonlooping�� -�   .��Clear animations�� -�   .��Reverse sequence�� .��Reverse all sequences�� -�   .��Mass create sequences...�� ${�False��.��Loop X Times...�� ${�False��.��Unloop�� -�   .��Clear translations�� -�   .��Clear scalings�� -�   .��Clear rotations�� -�   .��Clear all animations�� -�   .��Merge�� -�   .��Split�� -�   ${�False��.��Sync with...�� ${�False��.��Quick Animations...�� $��1��$��2��$��	0,208,0,0q�$��2��.+��� -�   $��InputGSDuration���� -�   .��Add�� -�   .��Edit�� -�   .��Remove�� $��Nodes��
�������.��� 
������� �� ��  �� $��20 ��� �� .+��� -�   $��Report_Nodes��$��20���� -�   $��header_nodedata��$��Node properties and data��$��20��$��0��$��1��R� -�   $��	ListNodes��$��0��$��1��$��2��
��������� -�   $��NodeDataEditor��$��1��$��1��.+��� $ 
Horizontal=�.+��� $��2q�.+��� -�   $��Check_Billlboarded��$��Billboarded���� -�   $��Check_Billlboardedx��$��Billboarded X���� -�   $��Check_Billlboardedy��$��Billboarded Y���� -�   $��Check_Billlboardedz��$��Billboarded Z���� $��2q�.+��� -�   $��Check_cameraAnchored��$��Camera Anchored���� -�   $��Check_dontInhTranslation��$��Dont inherit translation���� -�   $��Check_dontInhRotation��$��Dont inherit rotation���� -�   $��Check_DontinhScaling��$��Dont inherit scaling���� $ 
Horizontal=�$��0,5,0,0q�.+��� -�   $��ButtonEditTranslations��$��5,0,5,0q�$	��110��.��Translations�� -�   $��ButtonEditRotations��$��5,0,5,0q�$��90��.��	Rotations�� -�   $��ButtonEditScalings��$��5,0,5,0q�$��90��.��
Scalings�� $ 
Horizontal=�.+��� -�   $��InfoPivotPointNode��$	��200���� -�   $��50��$��5,0,5,0q�.��Set...�� -�   .��	Edit data�� -�   $��Report_Node_data��$��80�� �� 
IsReadOnly$
 True��
������� �� AcceptsReturn$
 True�� �� TextWrapping$
 Wrap=��� $��2��$��1��.���� .+��� -�   .��	Create...�� -�   �� -�   .��Rename�� -�   .��View Transformations�� -�   .��Delete�� -�   .��+)Delete all geometry attached to this bone�� -�   .��)'Set Pivot Point to Geoset's centroid...�� -�   .��Move to root�� -�   .��Move under node...�� -�   .��Root all nodes�� -�   .��Clear transformations...�� -�   .��Reverse Transformations...�� -�   $0	 *Makes the distance between keyframes equal��${�False��.��Space Transformations...�� -�   .��$"Clear all nodes of transformations�� -�   .��20Create all building/mechanical attachment points�� -�   .��#!Create all unit attachment points�� -�   .��Copy node transformations�� -�   .��paste node transformations�� -�   .��negate node position�� -�   .��Translate all nodes by....�� -�   .��Align nodes by....�� -�   .��center all nodes by....�� -�   .��#!Rotate all nodes collectively....�� -�   .��" Scale all nodes collectively....�� -�   .��*(Which geosets are attached to this bone?�� -�   .��/-Reverse rotations of this node's rotations...�� -�   .��$"Shift translations of this node...�� -�   .��)'shrink/inflate scalings of this node...�� $��Textures��
�������.��� 
������� �� ��  �� �� �� �� .+�� $��0��$��0�� �� $
��Auto ��� .+��� -�   $��Label_Materials��$��	Materials��
�������$	��Top=��� -�   $��List_MAterials��$��1��$
 Single=�
�������$��Stretch=�$��Stretch=��� $��0��$��1��$��10q�.+��� -�   $��Check_MatFS��$��Full Resolution���� -�   $��Check_MatSort��$��Sort Primitives Far Z���� -�   $��Check_MatCC��$��Constant Color���� -�   $	��New��$��5,0,5,0q��� -�   $��Delete��$��5,0,5,0q�� $��1��$��0�� �� $
��Auto ��� .+��� -�   $��Label_Layers��$��Layers��
�������$	��Top=��� -�   $��List_Layers��$��1��$
 Single=�
�������$��Stretch=�$��Stretch=� �� .@�m� -�   $��Move up��m� -�   $��	Move down��m� -�   $��
Create new���� $��1��$��1��.���� $��5q�.+��� -�   $��Check_Unshaded��$��Unshaded���� -�   $��Check_Unfogged��$��Unfogged���� -�   $��Check_twosided��$��Twosided���� -�   $��Check_sf��$��Sphere Env Map���� -�   $��Check_NoDepthTest��$��No Depth Test���� -�   $��Check_NoDepthSet��$��No Depth Set���� $ 
Horizontal=�.+��� $��Filter mode���� -�   $��Combo_FilterModeLayer��$	��150��@��� .��None�� .��Transparent�� .��Blend�� .��
Additive�� .��AdditiveAlpha�� .��
Modulate�� .��
Modulate2x�� $ 
Horizontal=�.+��� $��Using Texture���� -�   $��Combo_LayerUsedTexture��$	��150���� $ 
Horizontal=�.+��� $��Using Texture animation���� -�   $ ��Combo_LayerUsedTextureAnim��$	��150���� -�   $��Delete���� -�   $��View Transformations���� -�   $��
edit alpha���� -�   $��edit textuerid��� $��2��$��0�� �� $
��Auto ��� .+��� -�   $��LabelTextues��$��Textures���� -�   $��List_Textures��$��1��$
 Single=�
�������$��Stretch=�$��Stretch=��� $��2��$��1��$��3q�.���� .+��� -�   $��Find texture...���� -�   $(��"Mass create textures from paths...���� -�   $��Delete���� -�   $&�� Create material for this texture���� -�   $'��!Create materials for all textures���� -�   $��Create Team color���� -�   $��Create Team glow��� $��3��$��0�� �� $
��Auto ��� .+��� -�   $��LabelTextueAnims��$��Texture Animations���� -�   $��List_TextureAnims��
�������$��1��$
 Single=�$��Stretch=�$��Stretch=��� $��3��$��1��.+��� -�   $��Create���� -�   $��View transformations���� -�   $��edit translation���� -�   $��edit rotation���� -�   $��edit scaling���� -�   $��Delete���� $��	Inspector��$��Right=�
�������$��	0,-2,-2,0q�$��65��.���� -�   $��TC_Inspector��.@��� $
��Info��.��� 
������� �� �� .+��� $��0��.���� -�   $��LabelDisplayInfo���� $	��Top=�$��1��.+��� -�   .��#!Which sequences are not animated?�� -�   .��#!Copy sequences data for keyframes�� -�   .��%#show me sequence gaps up to 999,999�� $��Check for errors��.���� -�   $��
Box_Errors��
�������