   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   ,    &Wa3Tuner.Dialogs.ShatterAnimationMaker   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	     � 
ResizeMode$  NoResize=�5   	   
������� � Title$ Shatter Animation Maker��5
   	   $	��400��6)   $	��400��66   .��5      � .+�5   
   �� $��10q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��0 5 0 5q�62   .+�5      �� $��Target Geoset:��6   $��Center=�62   �� 5      -   5        $��TargetGeosetDropdown��5       �� 
IsReadOnly$
 True��66   
�������$	��200��6_   $��10 0 0 0q�6k   5      �� 5      $��Select Sequence(s):��6   �� 5      -   5        $��SequenceList��5      $	��100��6*    �� SelectionMode$ Multiple=�67   �� 5      $ 
Horizontal=�6   $��0 5 0 5q�62   .+�5      �� $��Travel Distance:��6   $��Center=�64   �� 5      -   5        $��ShatterDistanceInput��5      $��50��66   $��5q�6A   �� 5      $��-��6   $��Center=�6%   �� 5      -   5        $��ShatterDistanceInput2��5      $��50��67   $��5q�6B   �� 5      -   5        $��RandomizeDistanceCheck��5      $��	Randomize��69   $��10 0 0 0q�6M   5      �� 5"      -   5        $��ApplyRotationCheck��5"      $��Apply Random Rotation��61    d� 	IsChecked$
 TrueY�6Q   �� 5#      -   5        $��ApplyFallCheck��5#      $��Apply Fall to Ground��6-   $
 TrueY�6L   �� 5$      -   5        $��ApplyFadeCheck��5$      $��Apply Alpha Fade��6-   $
 TrueY�6H   �� 5'      
���  ��$��1q�6'   $��5q�6;    �� Padding$ 5q�6F   ._�5(      �� 
���  ��$
��Wrap=�6-   ��Warning: This operation creates a large number of bones and increases file size significantly. It will be difficult to edit later.5+      5,      �� 5/      $ 
Horizontal=�6   $��Right=�62   $��10 0 0 0q�6N   .+�50      �� -	   5        $��OkButton��50      $��Generate��64   $	��100��6G   $��5q�6S   �� 51      -
   5        $��CancelButton��51      $��Cancel��6-   $	��100��6M   $��5q�6Y   52      54      55      56      