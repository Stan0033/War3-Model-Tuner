   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.0.8.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	   $    Wa3Tuner.transformation_editor   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	     � WindowStartupLocation$  CenterScreen=�5   	   
������� � Title$ tranformation_editor��5
   	   $	��450��6&   $	��800��66   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��0,0,0,10q�6;   .+�5      	� -   5        $��RadioStatic��5      $��Static��6-    	� 	GroupName$ TransformationType��6>   $��0,0,10,0q�6]   6o   	� 5      -   5        $��RadioDynamic��5      $��Dynamic��6.   $ TransformationType��6@   6_   �� 5      -   5        $��Info_Keyframes��5       $�� ��6   $��	230,0,0,0q�66   5      �� 5    
   -   5        $��StaticInput��5       $��1��6%   $��25��62   $��0,0,0,10q�6>   �� 5#   
   -   5        $��MainList��5#      
������� �� SelectionMode$ Single=�69   $��2��6P   $��0,0,0,10q�6]   6�   �� 5&   
   -   5        $��stack0��5&      $��3��6#   $ 
Horizontal=�60   $��0,0,0,10q�6I   .+�5'      �� -   5        $��SequenceSelector��5'      $	��150��6/   $��0,0,5,0q�6;   6L   �� 5(      -   5        $��
TrackInput��5(      $��50��6(   $��0,0,5,0q�63   �� 5)      -	   5        $��
ValueInput��5)      $	��100��6(   $��0,0,5,0q�64   �� 5*      -
   5        $��ButtonColor��5*      $�� ��6(   $��20��63   $��0,0,5,0q�6>   6O   �� 5,      -   5        $��	ButtonNew��5,      $	��New��6&   $��50��64   $��0,0,5,0q�6?   6P   �� 5-      -   5        $��
ButtonEdit��5-      $
��Edit��6'   $��50��66   $��0,0,5,0q�6A   6R   �� 5.      -   5        $��	ButtonDel��5.      $��Delete��6&   $��50��67   $��0,0,5,0q�6B   6S   �� 50      -   5        $��
ButtonMore��50      $��More...��6'   $��50��69   $��0,0,5,0q�6D    � ContextMenu 51      �� 52      .@�55      m� -   5        $��Loop...��55   #   67   m� 56      -   5        $��	Clear all��56   %   69   m� 57      -   5        $��Reverse instructions��57   '   6G   m� 58      -   5        $��Leave only starts��58   (   6E   m� 59      -   5        $*��$Remove all of the selected sequence ��59   '   6V   m� 5:      -   5        $1��+Remove all except of the selected sequence ��5:   '   6\   m� 5;      -   5        $��Reverse times ��5;   &   6@   m� 5<      -   5        $ ��Leave only starts and ends��5<   &   6M   m� 5=      -   5        $ ��Create all starts and ends��5=   '   6N   m� 5>      -   5        $��Create all starts��5>   '   6D   m� 5?      -   5        $��Negate��5?   '   69   m� 5@      -   5        $��
Negate all��5@   '   6=   m� 5A      -   5        $��Set all to the input��5A   '   6G   m� 5B      -   5        $��Add the input to all��5B   '   6G   m� 5C      -   5        $!��Subtract the input from all��5C   '   6N   m� 5D      -   5        $!��Multiply all with the input��5D   '   6N   m� 5E      -   5        $��Divide all with the input��5E   '   6L   5G      5I      �� 5J      -    5        $+��%How transformations work in this app?��5J      $	��210��6E   $��0,0,5,0q�6Q   6b   5K      �� 5L   
   -!   5        $��stack1��5L      $ 
Horizontal=�6#   $��4��6<   .+�5M      �� $��Global Sequence��6   �� 5N      -"   5        $��Combo_GlobalSequence��5N      $��70��63   $��20��6>   $��8,0,0,0q�6J   6[   �� 5O      $��Interpolation Type��6   $��5,0,5,0q�62   �� 5P      -#   5        $��Combo_InterpType��5P      $��70��60   $��20��6;   $��8,0,0,0q�6G   @�5Q      �� 5R      .��6#   None6)   �� 5S      .��6#   Linear6+   �� 5T      .��6#   Bezier6+   �� 5U      .��6#   	Hermite6,   5W      5X      �� 5Z   
   -$   5        $��stack2��5Z      $ 
Horizontal=�6#   $��5��6<   5[      �� 5]   
   -%   5        $��4��5]      $��Update��6   $��Right=�6:   $��75��6V   5^      5_      