   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   ,    &Wa3Tuner.Dialogs.PivoPointOffsetWkndow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	     � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5	   	   
������� � Title$! Set Pivot point with offset��5   	   $	��220��6-   $	��290��6:   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Point:��6   $��0��6"   $��0��6/   $��5q�6?   �� 5   
    �� Orientation$ 
Horizontal=�6   $��0��6.   $��1��6;   .+�5      �� -   5        $��XInput��5      $��50��6$   $��2q�6/   �� 5      -   5        $��YInput��5      $��50��6$   $��2q�6/   �� 5      -   5        $��ZInput��5      $��50��6$   $��2q�6/   5      �� 5!   
   $��Axis:��6   $��1��6!   $��0��6.   $��5q�6>   �� 5"   
   -   5        $��AxisSelector��5"      $��1��6'   $��1��64   $	��150��6D   $
��Left=�6P    �� SelectedIndex$ 0��6k   .@�5#      �� $��X��6   �� 5$      $��Y��6   �� 5%      $��Z��6   5&      �� 5'   
   $��	Distance:��6   $��2��6%   $��0��62   $��5q�6B   �� 5(   
   -   5        $��DistanceInput��5(      $��2��6'   $��1��64   $	��100��6D   $
��Left=�6P   �� 5*   
   $��Angle (0-360):��6   $��3��6*   $��0��67   $��5q�6G   �� 5+   
   -   5        $��
AngleInput��5+      $��3��6$   $��1��61   $	��100��6A   $
��Left=�6M   �� 5-   
   -   5        $��OK��5-      $��4��6)   $��2��66   $	��150��6J   $
��Left=�6V   5.      5/      