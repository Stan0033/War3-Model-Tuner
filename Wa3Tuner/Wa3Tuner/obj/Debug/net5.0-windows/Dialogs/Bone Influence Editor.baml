   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   ,    &Wa3Tuner.Dialogs.Bone_Influence_Editor   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5          � 
ResizeMode$  NoResize=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	   
������� � Title$ Bone Influence ��5   	   $	��350��6!   $	��300��6.   .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5"   
   �� $!��Bone Weights (-100% - 100%)��6   $
��Bold"�67   $��16(�6I   $��0��6W   $��0��6d   $��4��6t   $��0,0,0,10q�6�    � ToolTip$j dBone weight determines how much and in which direction is the deformation of its vertices restricted��6�   �� 5$   
   $��X:��6   $��1��6   $��0��6+   $��Center=�6;   �� 5%   
   -   5        $��WeightX��5%      $��1��6!   $��1��6.   $��60��6>   $��5q�6I   �� 5'   
   $��Y:��6   $��2��6   $��0��6+   $��Center=�6;   �� 5(   
   -   5        $��WeightY��5(      $��2��6!   $��1��6.   $��60��6>   $��5q�6I   �� 5*   
   $��Z:��6   $��3��6   $��0��6+   $��Center=�6;   �� 5+   
   -   5        $��WeightZ��5+      $��3��6!   $��1��6.   $��60��6>   $��5q�6I   �� 5.   
   $��Constraints ('X' = any)��6   $
��Bold"�63   $��16(�6E   $��4��5/      $��0��6!   $��4��61   $��	0,10,0,10q�6E   $] WConstraints define how much nheriting transformation from its parent node is restricted��50      �� 52   
   $��X:��6   $��5��6   $��0��6+   $��Center=�6;   �� 53   
   -   5        $��ConstraintX1��53      $��X��6&   $��5��6/   $��1��6<   $��60��6L   $��5q�6W   �� 55   
   -   5        $��ConstraintX2��55      $��X��6&   $��5��6/   $��2��6<   $��60��6L   $��5q�6W   �� 56   
   $��-{float} - {float}��6   $��3��6.   $��5��6>   �� 57   
   $��0 - 360��6   $��3��6#   $��6��63   �� 58   
   $	��0+%��6   $��3��6   $��7��6/   �� 59   
   $��Y:��6   $��6��6   $��0��6+   $��Center=�6;   �� 5:   
   -   5        $��ConstraintY1��5:      $��0��6&   $��6��6/   $��1��6<   $��60��6L   $��5q�6W   �� 5;   
   -   5        $��ConstraintY2��5;      $	��360��6&   $��6��61   $��2��6>   $��60��6N   $��5q�6Y   �� 5=   
   $��Z:��6   $��7��6   $��0��6+   $��Center=�6;   �� 5>   
   -	   5        $��ConstraintZ1��5>      $��0��6&   $��7��6/   $��1��6<   $��60��6L   $��5q�6W   �� 5?   
   -
   5        $��ConstraintZ2��5?      $��X��6&   $��7��6/   $��2��6<   $��60��6L   $��5q�6W   �� 5B   
   -   5        $
��Save��5B      $��8��6+   $��4��68   $	��100��6J   $��Center=�5C      5D      5E      