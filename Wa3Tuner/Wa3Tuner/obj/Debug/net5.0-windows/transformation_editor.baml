   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.1.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	   $    Wa3Tuner.transformation_editor   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5	   	   
������� � Title$ tranformation_editor��5   	   $	��450��6&   $	��800��66   .��5      � -   5        $	��All��5      $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0��6.   $��0,0,0,10q�6;   .+�5      	� -   5        $��RadioStatic��5      $��Static��6-    	� 	GroupName$ TransformationType��6>   $��0,0,10,0q�6]   6o   	� 5      -   5        $��RadioDynamic��5      $��Animated��6.   $ TransformationType��6A   6`   �� 5      -   5        $��Info_Keyframes��5       $�� ��6   $��	230,0,0,0q�66   5      �� 5!   
   -   5        $��StaticInput��5!      $��1��6%   $��25��62   $��0,0,0,10q�6>   �� 5"   
   -   5        $��StaticInputColor��5"      $��1��6@   $��25��6M   $��0,0,0,10q�6Y   $y�	Collapsed=�6k   �� 5#   
   -   5        $��Check_StaticVisible��5#      $��1��6.   $��Visible��6<   $��Center=�6N   $y�	Collapsed=�6j   6�   �� 5%   
   -	   5        $��MainList��5%      
������� �� SelectionMode$ Single=�69   $��2��5&      $��0,0,0,10q�6   6R   �� 5)   
   -
   5        $��stack0��5)      $��3��6#   $ 
Horizontal=�60   $��0,0,0,10q�6I   .+�5*      �� -   5        $��SequenceSelector��5*      $	��150��6/   $��0,0,5,0q�6;   6L   �� 5+      -   5        $��
TrackInput��5+      $��50��6(   $��0,0,5,0q�63   �� 5,      -   5        $��
ValueInput��5,      $	��150��6(   $��0,0,5,0q�64   �� 5-      -   5        $��Check_InputVisible��5-      $��1��61   $y�	Collapsed=�6>   $��Visible��6U   $��Center=�6g   �� 5.      -   5        $��ButtonColor��5.      $�� ��6(   $��20��63   $y�	Collapsed=�6>   $��0,0,5,0q�6U   6f   �� 50      -   5        $��	ButtonNew��50      $	��New��6&   $��50��64   $��0,0,5,0q�6?   6P   �� 51      -   5        $��
ButtonEdit��51      $
��Edit��6'   $��50��66   $��0,0,5,0q�6A   6R   �� 52      -   5        $��	ButtonDel��52      $��Delete��6&   $��50��67   $��0,0,5,0q�6B   6S   �� 54      -   5        $��
ButtonCopy��54      $
��Copy��6'   $��50��66   $��0,0,5,0q�6A   6R   �� 55      -   5        $��ButtonPaste��55      $��Paste��6(   $��50��68   $��0,0,5,0q�6C   6T   �� 57      -   5        $��
ButtonMore��57      $��More...��6'   $��50��69   $��0,0,5,0q�6D    � ContextMenu 58      �� 59      .@�5<      m� -   5        $��Loop...��5<   #   67   m� 5=      -   5        $��	Clear all��5=   %   69   m� 5>      -   5        $��Reverse instructions��5>   '   6G   m� 5?      -   5        $��Leave only starts��5?   (   6E   m� 5@      -   5        $*��$Remove all of the selected sequence ��5@   '   6V   m� 5A      -   5        $1��+Remove all except of the selected sequence ��5A   '   6\   m� 5B      -   5        $��Reverse times ��5B   &   6@   m� 5C      -   5        $ ��Leave only starts and ends��5C   &   6M   m� 5D      -   5        $ ��Create all starts and ends��5D   '   6N   m� 5E      -   5        $��Create all starts��5E   '   6D   m� 5F      -    5        $��Negate��5F   '   69   m� 5G      -!   5        $��
Negate all��5G   '   6=   m� 5H      -"   5        $��Set all to the input��5H   '   6G   m� 5I      -#   5        $��Add the input to all��5I   '   6G   m� 5J      -$   5        $!��Subtract the input from all��5J   '   6N   m� 5K      -%   5        $!��Multiply all with the input��5K   '   6N   m� 5L      -&   5        $��Divide all with the input��5L   '   6L   5N      5P      �� 5Q      -'   5        $��
SetSpecial��5Q      $��Set per sequence...��6'   ${�False��6Y   $	��150��6k   $��0,0,5,0q�6w   5R      �� 5S   
   -(   5        $��stack1��5S      $ 
Horizontal=�6#   $��4��6<   .+�5T      �� $��Global Sequence��6   �� 5U      -)   5        $��Combo_GlobalSequence��5U      $��70��63   $��20��6>   $��8,0,0,0q�6J   6[   �� 5V      $��Interpolation Type��6   $��5,0,5,0q�62   �� 5W      -*   5        $��Combo_InterpType��5W      $��70��60   $��20��6;   $��8,0,0,0q�6G   @�5X      �� 5Y      .��6#   None6)   �� 5Z      .��6#   Linear6+   �� 5[      .��6#   Bezier6+   �� 5\      .��6#   	Hermite6,   5^      5_      �� 5a   
   -+   5        $��stack2��5a      $ 
Horizontal=�6#   $��5��6<   5b      �� 5d   
   -,   5        $��4��5d      $��Update��6   $��Right=�6D   $��75��6`   5e      5f      