   M S B A M L   `   `   `  ����  Wa3Tuner    Wa3Tuner.MainWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5        $��
App_window��5   '   
�������  � WindowStartupLocation$  CenterScreen=�5   	    � Title$ War3 Model Tuner��5
   	   $	��500��6"   $	��550��6/    � 	Resources 5       �� 
TargetType �� Property �� Value �� Triggers P� Property P� Value	 ]� 
Conditions
 �� Property �� Value��%e  '��       �� 5   
   " ��6   .�5      ��  � ��
 ���� 6+   �� 5       � ��
 �   �6+   �� 5       � ��$ 5q�6(   �� 5       � ��$ Stretch=�6;   �� 5       � ��$ Center=�69    5      P� 5       �@��
IsSelected$
 True��60   . �5      ��  � ��
 ����63   �� 5       � ��
 �   �63   5      ]� 5      	 5      �� 5      
 �@��
IsSelected$
 True��6:   �� 5      
 �@J�IsKeyboardFocusWithin$ False��6E   ./�5!      ��  � ��
 ��Zj�63   �� 5"       � ��
 ��  �63   5#      5%      5&      .��5'      �  � ColumnDefinitions 5(   
   �� 5)      �� 5*       � RowDefinitions 5,   
   �� 5-      $��20 �6   �� 5.      .+�52   
   o� $��0��6   
�������$��2��67   $��20��6K   .@�53      m� $��Model��6   .@�54      m� -   5        $
��Load��54       m� InputGestureText$ Ctrl+O��66   6R   m� 55      -   5        $��Reload��55      $ Ctrl+R��6;   6W   m� 56      -   5        $��
ButtonSave��56      $
��Save��6-   $ Ctrl+S��6J   6f   m� 57      -   5        $��ButtonSaveAs��57      $��
Save as...��6/   $ Alt+S��6S   6n   m� 58      -   5        $��ButtonOpenModelFolder��58      $��Open containing folder��68   6u   m� 59      -   5        $��Cameras��59      6-   m� 5:      -   5        $��Set model name��5:      64   5<      m� 5?      -	   5        $��Item_Recents��5?   (   $��Recents��6   6(   m� 5@      $��UI��6   .@�5B      m� -
   5        $��Refresh all lists��5B      $ Ctrl+F��6O   m� 5C      -   5        $��Make a screenshot��5C      67   m� 5D      -   5        $$��Make a screenshot of the model��5D      6D   5F      m� 5G      $��Render��6   .@�5H      m� -   5        $��Menuitem_Textured��5H       m� IsCheckable$
 True��64    m� 	IsChecked$
 True��6G   $��Textured��6X   $ Ctrl+T��5J      5K      m� 5L      
�������$��Import��6/   .@�5N      m� -   5        $��Import all geosets of...��5N   6   6[   m� 5O      -   5        $��Import all nodes of..��5O   4   6U   m� 5P      -   5        $��Import all textures of...��5P   2   6W   m� 5Q      -   5        $=��7Overwrite the list of sequences with another model's...��5Q   3   6v   5S      m� 5T      $��Cleaner��6   .@�5U      m� -   5        $��Remove all lights��5U      66   m� 5V      -   5        $��Remove all emitter 1s��5V      6:   m� 5W      -   5        $��Remove all emitter 2s��5W      6:   m� 5X      -   5        $��Remove all attachments��5X      6;   m� 5Y      -   5        $!��Remove all collision shapes��5Y      6@   m� 5Z      -   5        $��Remove all helpers��5Z      67   m� 5[      -   5        $��Remove all event objects��5[      6=   m� 5\      -   5        $��Remove all nodes��5\      65   m� 5]      -   5        $��Remove all geosets��5]      67   m� 5_      -   5        $#��Remove all texture animations��5_      6B   m� 5`      -   5        $��Remove all cameras��5`      67   m� 5a      -   5        $��Remove all sequences��5a      69   m� 5b      -   5        $!��Remove all global sequences��5b      6@   m� 5c      -   5        $��Remove all geosets��5c   1   m� 5d      -    5        $ ��Remove all node animations��5d      6?   5e      m� 5f      -!   5        $��MenuTeamColor��5f      $��
Team Color��6,   5h      m� 5i      -"   5        $��	UV Mapper��5i      $y�	Collapsed=�6:   m� 5j      $
��Help��6   .@�5k      m� -#   5        $��How a model works?��5k      68   m� 5l      -$   5        $"��How values work in this app?��5l      6B   m� 5m      -%   5        $��About��5m      6+   5n      5r      '� 5s   
   -&   5        $��Viewport_Main��5s      $��1��6*   $��0��67   $y�	Collapsed=�6G   5v      �� 5w   
   -'   5        $��ListOptions��5w      $��1��6(   $��0��65   $��2��6E   .@�5|      �� $��Optimize��6   $��2��6)   $��2��66   
�������.��5}      �  5~       5�      �� 5�      $��70 �6(   �� 5�      �� 5�      $��70 �6(   .+�5�      �� -(   5        $��Stack_Optimize��5�   !   
�������.+�5�      �� -)   5         � Cursor$
 Hand��5�   !   .��6F   	Check all6Q   �� 5�      -*   5        $
 Hand��5�   !   .��6G   Uncheck all6T   �� 5�      -+   5        $
 Hand��5�   !   .��6I   Inverse Check6X   5�      �� 5�      $��1��6#   $��2��60   .��5�      �� -,   5        $��ListOptOptions��5�   %   
�������.+�5�      ��  d� 	IsChecked$
 TrueY�6'   ${�False��68   .��6K   #!Re-arrange inconsistent sequences6n   �� 5�      $
 TrueY�6(   ${�False��69   .��6M   #!Re-arrange inconsistent keyframes6p   �� 5�      $
 TrueY�6'   ${�False��68   .��6J   add missing pivot points6d   �� 5�      $
 TrueY�6'   ${�False��68   .��6J    Fix invalid node relationships6j   �� 5�      $
 TrueY�6'   ${�False��68   .��6J   Remove empty geosets6`   �� 5�      $
 TrueY�6'   ${�False��68   .��6J   64Give/create missing components to components without6�   �� 5�      $
 trueY�6'   ${�False��68   .��6J   " Remove invalid geoset animations6l   �� 5�      $
 trueY�6'   ${�False��68   .��6J   ;9Make dynamic transformations with one/no keyframes static6�   �� 5�      $
 trueY�6'   ${�False��68   .��6J   " Remove invalid geoset animations6l   �� 5�      --   5        $��Check_Linearize��5�   '   .��6>   Linearize animations6T   �� 5�      -.   5        $��Check_delEmptyEO��5�   '   .��6?   Delete empty event objects6[   �� 5�      -/   5        $ ��check_delISolatedTriangles��5�   '   .��6I   Delete isolated triangles6d   �� 5�      -0   5        $��check_delISolatedVertices��5�   '   .��6H   Delete isolated vertices6b   �� 5�      -1   5        $��check_noArea��5�   '   .��6;   Delete triangles with no area6Z   �� 5�      -2   5        $��check_delOverlapping1��5�   '   .��6G   @>Delete fully overlapping triangles who share the same vertices6�   �� 5�      -3   5        $��check_delOverlapping2��5�   '   .��6F   A?Delete fully overlapping triangles who share different vertices6�   �� 5�      -4   5        $��check_mergevertices��5�   '   .��6E   ?=Merge vertices with identical positions and texture positions6�   �� 5�      -5   5        $��check_mergeTextures��5�   '   .��6E   Merge identical textures6_   �� 5�      -6   5        $��check_mergeMaterials��5�   '   .��6F   Merge identical materials6a   �� 5�      -7   5        $��check_mergeLayers��5�   '   .��6C   delete duplicate layers6\   �� 5�      -8   5        $��check_mergeTAs��5�   '   .��6@   $"Merge identical texture animations6d   �� 5�      -9   5        $"��Check_Delete0LengthSequences��5�   '   .��6K   Delete 0-length sequences6f   �� 5�      -:   5        $#��Check_Delete0LengthGSequences��5�   '   .��6L   " Delete 0-length global sequences6n   �� 5�      -;   5        $%��Check_DeleteUnanimatedSequences��5�   '   .��6N   Delete un-animated sequences6l   �� 5�      -<   5        $��Check_delunusedgs��5�   '   .��6@   Delete unused global sequencs6_   �� 5�      -=   5        $��Check_ConvertBones��5�   '   .��6A   ?=Remove bones with no attached vertices to it, and no children6�   �� 5�      ->   5        $��Check_DeleteUnusedHelpers��5�   '   .��6H   Delete childless helpers6b   �� 5�      -?   5        $!��Check_DeleteUnusedMaterials��5�   '   .��6J   Delete unused materials6c   �� 5�      -@   5        $ ��Check_DeleteUnusedTextures��5�   '   .��6I   Delete unused textures6a   �� 5�      -A   5        $)��#Check_DeleteUnusedTextureAnimations��5�   '   .��6R   " Delete unused texture animations6t   �� 5�      -B   5        $!��Check_DeleteUnusedKeyframes��5�   '   .��6J   Delete unused keyframes6c   �� 5�      -C   5        $-��'Check_DeleteIdenticalAdjascentKEyframes��5�   '   .��6V   &$Delete identical adjascent keyframes6|   �� 5�      -D   5        $3��-Check_DeleteIdenticalAdjascentKEyframes_times��5�   '   .��6\   .,Delete identical adjascent keyframes's times6�   �� 5�      -E   5        $)��#Check_DeleteSimilarSimilarKEyframes��5�   '   ${�False��6R   .��6e   $"Delete similar adjascent keyframes6�   �� 5�      -F   5        $��Check_MissingKeyframes��5�   '   .��6E   Add missing opening keyframes6d   �� 5�      -G   5        $��Check_MergeGeosets��5�   '   .��6A   +)Merge geosets that use the same materials6l   �� 5�      -H   5        $$��Check_DeleteUnusedMatrixGroups��5�   '   .��6M   Delete unused matrix groups6j   �� 5�      -I   5        $��Check_Minimiematrixgroups��5�   '    � ToolTip$b \If two or more matrix groups contain the same nodes regardless of order, they will be merged��6H   .��6�   " Minimize repeating matrix groups6�   �� 5�      -J   5        $��Check_CalculateExtents��5�   '   .��6E   Recalculate extents6Z   �� 5�      -K   5        $��Check_ClampKeyframes��5�   '   .��6C   clamp all keyframes6X   �� 5�      -L   5        $��Check_ClampUVs��5�   '   .��6=   clamp all uv coordinates6W   �� 5�      -M   5        $��Check_ClampLights��5�   '   .��6@   clamp all light attentuations6_   �� 5�      -N   5        $��Check_ClampNormals��5�   '   .��6A   clamp invalid normals6X   �� 5�      -O   5        $��Check_AddMissingOrigin��5�   '   .��6E   add origin ref if missing6`   �� 5�      -P   5        $��Check_DeleteDuplciateGAs��5�   '   .��6G   $"delete duplicate geoset animations6k   �� 5�      -Q   5        $"��Check_DeleteDuplciateGeosets��5�   '   .��6K   delete duplicate geosets6e   �� 5�      -R   5        $!��Check_AddMissingGeosetAnims��5�   '   .��6J   add missing geoset animations6i   �� 5�      -S   5        $��Check_SetAllStaticGAs��5�   '   .��6D   -+set all alpha static geoset animations to 16q   �� 5�      -T   5        $"��Check_AddMissingVisibilities��5�   '   .��6K   =;add missing visibilities to all geoset animations and nodes6�   �� 5�      -U   5        $��Check_DetachFromNonBone��5�   '   .��6F   IGdetach vertices from non-bone nodes and if empy, reattach to dummy bone6�   �� 5�      -V   5        $��Check_VisInterp��5�   '   .��6>   +)set interpolation of visibilities to none6i   5�      5�      �� 5�      -W   5        $
 Hand��5�      $��2��6=   $��2��6J   ���	#FFDDDDDD.��6w   
Optimize5�      5�      5�      �� 5�      $��Geosets��6   
�������.��5�      � 
������� 5�      �� 5�      $��* �6+   �� 5�      $	��200��6,   $	��450��6;    5�      �� 5�      $��20 �6(   �� 5�      $��* �6(   �� 5�      $��25 �6(   �� 5�      $��* �6(   .+�5�      '� -X   5        $��Scene_Viewport��5�   $   $��0��6:   $��4��6J   �� 5�      -Y   5        $��SceneOVerlay��5�      $��0��62   $��4��6B   
������ 5�      �� 5�       �� Orientation$ Vertical=�6!   $��1��68   $��0��6E   �� 5�      $ 
Horizontal=�6!   $��0��6;   $��1��6H   .+�5�      �� -Z   5        $��Report_Geosets��5�   $   $��5,0,5,0q�6:   $��Geosets��6K   $
��Bold"�6Z   $��1��6m   $��0��6}   �� 5�      $ 
Horizontal=�6%   $��1��6@   $��0��6Q   .+�5�      �� -[   5        $��ButtonGeosetOptions��5�   5   $
 Hand��6%   $	��...��6_   $��30��6m   $
��Bold"�6x   $	��Top=�6�   
������� � ContextMenu 5�   "   �� 5�   &   .@�5�   *   m� -\   5        $��Edit visibilities��5�   3   6N   m� 5�   *   -]   5        $��Change used material��5�   3   6Q   m� 5�   *   -^   5        $��Go to material��5�   3   6K   m� 5�   *   -_   5        $��
Go to bone��5�   3   6G   m� 5�   *   $��Normals��63   .@�5�   .   m� -`   5        $��Average normals��5�   7   6P   m� 5�   .   -a   5        $��Negate normals��5�   7   6O   5  +   m� 5  *   $��Geoset Operations��63   .@�5  .   m� $(��"Center                            ��67   .@�5  2   m� -b   5        $��	Center...��5  ;   6N   m� 5  2   -c   5        $��Center at...��5  ;   6Q   5  /   m� 5	  .   $��
Flatten...��67   .@�5
  2   m� -d   5        $��
Flatten...��5
  ;   6O   m� 5  2   -e   5        $��Flatten side...��5  ;   6T   5  /   m� 5  .   -f   5        $��Put on ground...��5  7   6Q   m� 5  .   -g   5        $��Translate...��5  7   6M   m� 5  .   -h   5        $��	Rotate...��5  7   6J   m� 5  .   -i   5        $��Scale each...��5  7   6N   m� 5  .   -j   5        $��	Negate...��5  7   6J   m� 5  .   -k   5        $��Merge��5  7   $O IAll selected geosets will be merged into one and use the first's material��6F   6�   m� 5  .   -l   5        $��Separate��5  7   $M Gif a geoset contains 2 or more meshes, split them into different gesets��6I   6�   m� 5  .   -m   5        $��Pull together...��5  7   6Q   m� 5  .   -n   5        $��	Subdivide��5  7   $K ESplits each triangle into 3 triangles. Preserves texture coordinates.��6J   6�   m� 5  .   -o   5        $��Simplify��5  7   $u oFor each flat surface found inside the selected geoset, reduces it to the minimum required triangles to form it��6I   6�   m� 5  .   -p   5        $��	Duplicate��5  7   6J   m� 5  .   -q   5        $��Align...��5  7   6I   m� 5  .   -r   5        $��Scale to fit...��5  7   6P   m� 5  .   -s   5        $,��&Create collision shape for this geoset��5  7   6g   m� 5  .   -t   5        $��Fragment into geosets��5  7   $S MSplits a geoset into multiple geosets, each holding a single of its triangles��6V   6�   m� 5  .   -u   5        $��Fragment inside geoset��5  7   $3 -Every triangle in the geoset will be detached��6W   6�   m� 5  .   -v   5        $!��Fragment faces into geosets��5  7   $] WSplits a geoset into multiple geosets, each holding a single of its faces/flat surfaces��6\   6�   m� 5  .   -w   5        $��Fragment faces in geoset��5  7   $. (detaches each flat surface in the geoset��6Y   6�   m� 5   .   -x   5        $��Weld all��5   7   $> 8Merges all vertices that overlap of each selected geoset��6I   6�   5!  +   m� 5"  *   $��
Attachment��63   .@�5#  .   m� -y   5        $3��-Reattach selected geoset/s to another bone...��5#  7   6n   m� 5$  .   -z   5        $,��&Reattach all geoset to another bone...��5$  7   6g   5&  +   m� 5'  *   $��Create��63   .@�5(  .   m� -{   5        $&�� Create Hero team glow (and bone)��5(  7   6a   m� 5)  .   -|   5        $'��!Create Team background (and bone)��5)  7   6b   m� 5+  .   $��Create Primitive shape...��67   .@�5,  2   m� -}   5        $
��Cube��5,  ;   6I   m� 5-  2   -~   5        $��Cylinder��5-  ;   6M   m� 5.  2   -   5        $
��Cone��5.  ;   6I   5/  /   m� 50  .   -�   5        $��Draw...��50  7   $y�	Collapsed=�6Z   m� 51  .   -�   5        $ ��Create extruded polygon...��51  7   $y�	Collapsed=�6s   52  +   m� 53  *   $��UV��63   .@�54  .   m� -�   5        $��	Clamp All��54  7   6J   m� 55  .   -�   5        $��	Negate Us��55  7   6J   m� 56  .   -�   5        $��	Negate Vs��56  7   6J   m� 57  .   -�   5        $��Swap Us and Vs��57  7   6O   m� 58  .   -�   5        $	��Fit��58  7   $\ VIf the geoset has 2 triangles, their textue coordintes will be fitted onto the texture��6W   59  +   5:  '   5<     �� 5=     -�   5        $
 Hand��5=  %   $	��All��64   $��30��6[   $ 
Select all��6f   �� 5>     -�   5        $
 Hand��5>  %   $
��None��64   $��35��6_   $ Select none��6j   �� 5?     -�   5        $
 Hand��5?  %   $��Invert��64   $��40��6b   $ Invert select��6m   �� 5@     -�   5        $��
ButtonDelG��5@  4   $
 Hand��6%   $	��Del��6F   $��30��6g   $��0��6t   5B     5C     �� 5D     -�   5        $��ListGeosets��5D  5   
�������$��1��6I   $��1��5E      �� SelectionMode$ Multiple=�6+   6D   �� 5G     -�   5        $��List_GeosetAnims��5G  7   
�������$��3��6O   $��1��6\   $ Single=�5H     �� 5M     $ 
Horizontal=�6!   $��2��6;   $��1��6H   
�������.+�5N     �� -�   5        $��	Label_GAs��5N  $   $��Geoset animations��65   $
��Bold"�6N   $��0,0,5,0q�6`   �� 5O     -�   5        $��	Buttongam��5O  0   $
 Hand��6!   $	��...��6Q   $��30��6_   $
��Bold"�6k   $	��Top=�5P     
������� 5Q     �� 5R  "   .@�5S  &   m� -�   5        $��view transformations��5S  /   6M   m� 5T  &   -�   5        $��Make use/not use color��5T  /   6O   m� 5U  &   -�   5        $-��'Make all geoset animations alpha static��5U  /   6`   m� 5V  &   -�   5        $@��:equalize the alpha animation data of all geoset animations��5V  /   6s   m� 5W  &   -�   5        $@��:equalize the color animation data of all geoset animations��5W  /   6s   m� 5X  &   -�   5        $1��+Make all geoset animations the same as this��5X  /   6d   m� 5Y  &   -�   5        $5��/Reset all geoset animations' alphas to static 1��5Y  /   6h   m� 5Z  &   -�   5        $��Create missing��5Z  /   6G   m� 5[  &   -�   5        $��
Delete all��5[  /   6C   5\  #   5^     �� 5_     -�   5        $
 Hand��5_  !   $��30��60   $ Edit Alphas��6;   .��6h   A6k   �� 5`     -�   5        $
 Hand��5`  !   $��30��60   $ 
Edit Color��6;   .��6h   C6k   5a     5h     5j     �� 5l     $��	Sequences��6   
�������.��5m     � 
������� 5n     �� 5o     $
��203* �6+   �� 5p     $	��69* �6+   �� 5q     $
��272* �6+    5s     �� 5t     $��20 �6(   �� 5u     �� 5v     �� 5w     $��80 �6(   .+�5z     �� -�   5        $��Report_sequences��5z      $��0��68   $��0��6E   $��2��6U   �� 5{     -�   5        $��Report_gsequences��5{      $��0��69   $��2��6F   �� 5|     -�   5        $��ListSequenes��5|     
�������$ Single=�6N   $��1��5}     $��2��6+   6g   �� 5     -�   5        $��ListGSequenes��5     
�������$��1��6j   $��2��6w   $ Single=�6�   �� 5�     $��Center=�6#   $��0��6>   $��2��6P   $��2��6]   .��5�     �� 
�������$
��1000��6=   .+�5�     �� -�   5        $
 Hand��5�  %   .��6I   New6N   �� 5�     -�   5        $
 Hand��5�  %   .��6J   Create presets....6^   �� 5�     -�   5        $
 Hand��5�  %   .��6K   Rename6S   �� 5�     -�   5        $
 Hand��5�  %   .��6F   Swap names with...6Z   �� 5�     -�   5        $
 Hand��5�  %   .��6c   %#Duplicate (with matching keyframes)6�   �� 5�     -�   5        $
 Hand��5�  %   .��6K   " Resize... (along with keyframes)6m   �� 5�     -�   5        $
 Hand��5�  %   .��6H   #!Delete (along with all keyframes)6k   �� 5�     -�   5        $
 Hand��5�  %   .��6J   ,*Which bones are animated in this sequence?6v   �� 5�     -�   5        $
 Hand��5�  %   .��6J   Make looping/Nonlooping6c   �� 5�     -�   5        $
 Hand��5�  %   .��6T   Clear animations6f   �� 5�     -�   5        $
 Hand��5�  %   .��6L   Reverse sequence6^   �� 5�     .��6%   Reverse all sequences6<   �� 5�     -�   5        $
 Hand��5�  %   .��6Q   Mass create sequences...6k   �� 5�     -�   5        $
 Hand��5�  %   .��6V   Clear translations6j   �� 5�     -�   5        $
 Hand��5�  %   .��6R   Clear scalings6b   �� 5�     -�   5        $
 Hand��5�  %   .��6S   Clear rotations6d   �� 5�     -�   5        $
 Hand��5�  %   .��6Y   Clear all animations6o   �� 5�     -�   5        $
 Hand��5�  %   .��6S   Merge6Z   �� 5�     -�   5        $
 Hand��5�  %   .��6K   Split6R   �� 5�     -�   5        $
 Hand��5�  %   $� For each component's transformation, if that transformation's keyframes count is equal to the second sequence, sync their times��6H   .��6�   Sync with...6�   5�     5�     �� 5�     $��1��6!   $��2��6.   $��	0,208,0,0q�6>   $��2��6Q   .+�5�     �� -�   5        $��InputGSDuration��5�  "   6;   �� 5�     -�   5        $
 Hand��5�  !   .��6>   Add6C   �� 5�     -�   5        $
 Hand��5�  !   .��6?   Edit6E   �� 5�     -�   5        $
 Hand��5�  !   .��6>   Remove6F   5�     �� 5�     -�   5        $��Report_Sequence_UsedNodes��5�     $��0��6?   $��2��6O   $��3��6c    �� 
IsReadOnly$
 True��6p    �� AcceptsReturn$
 True��6�    �� 
AcceptsTab$
 True��5�      �� TextWrapping$
 Wrap=�5�     " �� VerticalScrollBarVisibility$
 Auto=�5�     $ �� HorizontalScrollBarVisibility$ Disabled=�5�     �� 5�     -�   5        $ ��Report_GSequence_UsedNodes��5�     $��2��6@   $��3��6Q   $
 True��6^   $
 True��6q   $
 True��5�     $
 Wrap=�5�     $
 Auto=�5�     $ Disabled=�5�     5�     5�     �� 5�     $��Nodes��6   
�������.��5�     � 
������� 5�     �� 5�     �� 5�      5�     �� 5�     $��20 �6(   �� 5�     �� 5�     .+�5�     �� -�   5        $��Report_Nodes��5�      $��20��64   �� 5�     -�   5        $��header_nodedata��5�      $��Node properties and data��67   $��20��6W   $��0��6c   $��1��6p   R� 5�     -�   5        $��	ListNodes��5�     $��0��60   $��1��6@   $��2��6m   
�������5�  =   �� 5�     -�   5        $��NodeDataEditor��5�  !   $��1��67   $��1��6G   .+�5�     �� $ 
Horizontal=�6%   .+�5�     �� $��2q�6)   .+�5�  "   �� -�   5        $��Check_Billlboarded��5�  A   $��Billboarded��6+   6{   �� 5�  "   -�   5        $��Check_Billlboardedx��5�  C   $��Billboarded X��6+   6~   �� 5�  "   -�   5        $��Check_Billlboardedy��5�  C   $��Billboarded Y��6+   6~   �� 5�  "   -�   5        $��Check_Billlboardedz��5�  C   $��Billboarded Z��6+   6~   5�     �� 5�     $��2q�6)   .+�5�  "   �� -�   5        $��Check_cameraAnchored��5�  E   $��Camera Anchored��6+   6�   �� 5�  "   -�   5        $��Check_dontInhTranslation��5�  N   $��Dont inherit translation��6+   6�   �� 5�  "   -�   5        $��Check_dontInhRotation��5�  K   $��Dont inherit rotation��6+   6�   �� 5�  "   -�   5        $��Check_DontinhScaling��5�  J   $��Dont inherit scaling��6+   6�   5�     5�     �� 5�     $ 
Horizontal=�6%   $��0,5,0,0q�6>   .+�5�     �� -�   5        $��ButtonEditTranslations��5�  4   $
 Hand��6%   $��5,0,5,0q�6o   $	��110��6�   .��6�   Translations6�   �� 5�     -�   5        $��ButtonEditRotations��5�  4   $
 Hand��6%   $��5,0,5,0q�6i   $��90��6z   .��6�   	Rotations6�   �� 5�     -�   5        $��ButtonEditScalings��5�  4   $
 Hand��6%   $��5,0,5,0q�6g   $��90��6x   .��6�   
Scalings6�   5�     �� 5�     $ 
Horizontal=�6%   .+�5�     �� -�   5        $��InfoPivotPointNode��5�  (   $	��200��6B   �� 5�     -�   5        $
 Hand��5�  %   $��50��6J   $��5,0,5,0q�6U   .��6f   Set...6n   5�     �� 5�     -�   5        $
 Hand��5�  !   .��6E   	Edit data6P   �� 5�     -�   5        $��Report_Node_data��5�  .   $��80��6"   $
 True��6F   
�������$
 True��6o   $
 Wrap=�5�     5�     �� 5�     $��2��6%   $��1��62   .��5�     �� .+�5�     �� -�   5        $
 Hand��5�  %   .��6N   	Create...6Y   �� 5�     $
 Hand��6%   $��
Presets...��65   $y�	Collapsed=�6J    5�  "   �� 5�  &   .@�5�  *   m� -�   5        $��Item Pixies��5�  3   
�������6]   m� 5�  *   -�   5        $
��Dust��5�  3   
�������6T   m� 5�  *   -�   5        $��Smoke��5�  3   
�������6V   5�  '   5�     �� 5�     -�   5        $
 Hand��5�  %   .��6G   Rename6O   �� 5�     -�   5        $
 Hand��5�  %   .��6E   Find by name...6V   �� 5�     -�   5        $
 Hand��5�  %   .��6T   View Transformations6j   �� 5�     -�   5        $
 Hand��5�  %   .��6D   Delete6L   �� 5�     -�   5        $
 Hand��5�  %   .��6M   +)Delete all geometry attached to this bone6x   �� 5     -�   5        $
 Hand��5  %   .��6C   )'Set Pivot Point to Geoset's centroid...6l   �� 5     -�   5        $
 Hand��5  %   .��6G   Move to root6U   �� 5     -�   5        $
 Hand��5  %   .��6T   Move under node...6h   �� 5     -�   5        $
 Hand��5  %   .��6J   	Duplicate6U   �� 5     -�   5        $
 Hand��5  %   .��6D   Root all nodes6T   �� 5     -�   5        $
 Hand��5  %   .��6c   #!Remove sequenes from keyframes...6�   �� 5     -�   5        $
 Hand��5  %   .��6K    Reset all keyframes to default6k   �� 5     -�   5        $
 Hand��5  %   .��6K   Clear transformations...6e   �� 5	     -�   5        $
 Hand��5	  %   .��6J   Reverse Transformations...6f   �� 5
     -�   5        $
 Hand��5
  %   $0 *Makes the distance between keyframes equal��6F   .��6}   Space Transformations...6�   �� 5     -�   5        $
 Hand��5  %   .��6N   $"Clear all nodes of transformations6r   �� 5     -�   5        $
 Hand��5  %   .��6H   20Create all building/mechanical attachment points6z   �� 5     -�   5        $
 Hand��5  %   .��6H   #!Create all unit attachment points6k   �� 5     -�   5        $
 Hand��5  %   .��6Q   Copy node transformations6l   �� 5     -�   5        $
 Hand��5  %   .��6R   paste node transformations6n   �� 5     -�   5        $
 Hand��5  %   .��6N   negate node position6d   �� 5     -�   5        $
 Hand��5  %   .��6M   Translate all nodes by....6i   �� 5     -�   5        $
 Hand��5  %   .��6G   Align nodes by....6[   �� 5     -�   5        $
 Hand��5  %   .��6K   center all nodes by....6d   �� 5     -�   5        $
 Hand��5  %   .��6X   #!Rotate all nodes collectively....6{   �� 5     -�   5        $
 Hand��5  %   .��6I   " Scale all nodes collectively....6k   �� 5     -�   5        $
 Hand��5  %   .��6M   *(Which geosets are attached to this bone?6w   �� 5     -�   5        $
 Hand��5  %   .��6[   /-Reverse rotations of this node's rotations...6�   �� 5     -�   5        $
 Hand��5  %   .��6T   $"Shift translations of this node...6x   �� 5     -�   5        $
 Hand��5  %   .��6S   )'shrink/inflate scalings of this node...6|   5     5     5!     5#     �� 5$     $��Textures��6   
�������.��5%     � 
������� 5&     �� 5'     �� 5(      5*     �� 5+     �� 5,     �� 5-     �� 5.     .+�52     � $��0��6   $��0��6(    53     �� 54     $
��Auto �6,   �� 55     .+�57     �� -�   5        $��Label_Materials��57  $   $��	Materials��6;   
�������$	��Top=�6c   �� 58     -�   5        $��List_MAterials��58  "   $��1��68   $ Single=�6E   
�������$��Stretch=�5:     $��Stretch=�64   5;     �� 5<     $��0��6!   $��1��6.   $��10q�6>   .+�5=     �� -�   5        $��Check_MatFS��5=  #   $��Full Resolution��66   6m   �� 5>     -�   5        $��Check_MatSort��5>  #   $��Sort Primitives Far Z��68   6r   �� 5?     -�   5        $��Check_MatSort2��5?  #   $��Sort Primitives Near Z��69   6u   �� 5@     -�   5        $��Check_MatCC��5@  #   $��Constant Color��66   6g   �� 5A     -�   5        $
 Hand��5A  !   $	��New��6G   $��5,0,5,0q�6U   �� 5B     -�   5        $
 Hand��5B  !   $��Delete��6D   $��5,0,5,0q�6U   5C     � 5F     $��1��6   $��0��6(    5G     �� 5H     $
��Auto �6,   �� 5I     .+�5K     �� -�   5        $��Label_Layers��5K  $   $��Layers��68   
�������$	��Top=�6]   �� 5L     -�   5        $��List_Layers��5L  "   $��1��65   $ Single=�6B   
�������$��Stretch=�5N     $��Stretch=�64    5O     �� 5P  "   .@�5Q  &   m� -�   5        $��Move up��5Q  /   6@   m� 5R  &   -�   5        $��	Move down��5R  /   6B   m� 5S  &   -�   5        $��
Create new��5S  /   6C   5T  #   5V     5W     �� 5X     $��1��6#   $��1��60   .��5Y     �� $��5q�6%   .+�5Z     �� -�   5        $��Check_Unshaded��5Z  '   $��Unshaded��6=   6k   �� 5[     -�   5        $��Check_Unfogged��5[  '   $��Unfogged��6=   6k   �� 5\     -�   5        $��Check_twosided��5\  '   $��Twosided��6=   6k   �� 5]     -�   5        $��Check_sf��5]  '   $��Sphere Env Map��67   6e   �� 5^     -�   5        $��Check_NoDepthTest��5^  '   $��No Depth Test��6@   6v   �� 5_     -�   5        $��Check_NoDepthSet��5_  '   $��No Depth Set��6?   6s   �� 5`     $ 
Horizontal=�6)   .+�5a  "   �� $��Filter mode��6,   $��80��6?   �� 5b  "   -�   5        $��Combo_FilterModeLayer��5b  +   $	��150��6H   @�5c  &   �� 5d  *   .��67   None6=   �� 5e  *   .��67   Transparent6D   �� 5f  *   .��67   Blend6>   �� 5g  *   .��67   
Additive6A   �� 5h  *   .��67   AdditiveAlpha6F   �� 5i  *   .��67   
Modulate6A   �� 5j  *   .��67   
Modulate2x6C   5n  #   5p     �� 5q     $ 
Horizontal=�6)   .+�5r  "   �� $��Using Texture��6,   $��80��6B   �� 5s  "   -   5        $��Combo_LayerUsedTexture��5s  +   $	��150��6I   6U   5u     �� 5v     $ 
Horizontal=�6)   .+�5w  "   �� $��Using Texture animation��6,   $	��140��6K   �� 5x  "   -  5        $ ��Combo_LayerUsedTextureAnim��5x  +   $	��150��6M   5y  +   5{     �� 5}     -  5        $
 Hand��5}  %   $��Delete��6E   �� 5~     -  5        $
 Hand��5~  %   $��View Transformations��6U   �� 5     -  5        $
 Hand��5  %   $��
edit alpha��6K   �� 5�     -  5        $
 Hand��5�  %   $��edit textuerid��6O   5�     5�     � 5�     $��2��6   $��0��6(    5�     �� 5�     $
��Auto �6,   �� 5�     .+�5�     �� -  5        $��LabelTextues��5�  $   $��Textures��68   �� 5�     -  5        $��List_Textures��5�  "   $��1��67   $ Single=�6D   
�������$��Stretch=�5�     $��Stretch=�64   6P   5�     �� 5�     $��2��6$   $��1��61   $��3q�6A   .��5�     �� .+�5�     �� -  5        $
 Hand��5�  %   $��Find texture...��6H   �� 5�     -	  5        $
 Hand��5�  %   $(��"Mass create textures from paths...��6K   �� 5�     -
  5        $
 Hand��5�  %   $��Delete��6G   �� 5�     -  5        $
 Hand��5�  %   $&�� Create material for this texture��6[   �� 5�     -  5        $
 Hand��5�  %   $'��!Create materials for all textures��6U   �� 5�     -  5        $
 Hand��5�  %   $��Create Team color��6E   �� 5�     -  5        $
 Hand��5�  %   $��Create Team glow��6E   �� 5�     -  5        $��Check_WW��5�  '   $��
Wrap Width��67   6b   �� 5�     -  5        $��Check_WH��5�  '   $��Wrap Height��67   6c   5�     5�     � 5�     $��3��6   $��0��6(    5�     �� 5�     $
��Auto �6,   �� 5�     .+�5�     �� -  5        $��LabelTextueAnims��5�  $   $��Texture Animations��6<   �� 5�     -  5        $��List_TextureAnims��5�  "   
�������$��1��6V   $ Single=�6c   $��Stretch=�5�     $��Stretch=�64   5�     �� 5�     $��3��6!   $��1��6.   .+�5�     �� -  5        $
 Hand��5�  !   $��Create��6A   �� 5�     -  5        $
 Hand��5�  !   $��View transformations��6I   �� 5�     -  5        $
 Hand��5�  !   $��edit translation��6C   �� 5�     -  5        $
 Hand��5�  !   $��edit rotation��6C   �� 5�     -  5        $
 Hand��5�  !   $��edit scaling��6H   �� 5�     -  5        $
 Hand��5�  !   $��Delete��6J   5�     5�     5�     �� 5�     $��	Inspector��6   $��Right=�6)   
�������$��	0,-2,-2,0q�6]   $��65��6p   .��5�     �� -  5        $��TC_Inspector��5�     .@�5�     �� $
��Info��6   .��5�     � 
������� 5�     �� 5�  "   �� 5�  "   .+�5�     �� $��0��6+   .��5�  "   �� -  5        $��LabelDisplayInfo��5�  ,   5�     �� 5�     $	��Top=�6)   $��1��6A   .+�5�  "   �� -  5        $
 Hand��5�  )   .��6R   #!Which sequences are not animated?6u   �� 5�  "   -  5        $
 Hand��5�  )   .��6K   #!Copy sequences data for keyframes6n   �� 5�  "   -  5        $
 Hand��5�  )   .��6I   %#show me sequence gaps up to 999,9996n   5�     5�     5�     �� 5�     $��Check for errors��6   .��5�     �� -  5        $��
Box_Errors��5�  "   
�������$ Visible=�6L   5�     5�     5�     5�     5�     5�     