   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.SCALETOFIT   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � Title$ Scale to fit - Extent��5   	   $	��320��6'   $	��300��64   .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0��6%   $��5q�62   .+�5      �� $��
Minimum X:��6   �� 5      -   5        $��	MinXInput��5      $��0,0,0,10q�6'   �� 5      $��
Minimum Y:��6   �� 5      -   5        $��	MinYInput��5      $��0,0,0,10q�6'   �� 5      $��
Minimum Z:��6   �� 5      -   5        $��	MinZInput��5      5       �� 5"   
   $��1��6   $��0��6%   $��5q�62   .+�5#      �� $��
Maximum X:��6   �� 5$      -   5        $��	MaxXInput��5$      $��0,0,0,10q�6'   �� 5%      $��
Maximum Y:��6   �� 5&      -   5        $��	MaxYInput��5&      $��0,0,0,10q�6'   �� 5'      $��
Maximum Z:��6   �� 5(      -   5        $��	MaxZInput��5(      5)      �� 5+   
   $��2��6   $��1��6)    �� Orientation$ Vertical=�66   $��5q�6M   .+�5,      	� -   5        $��SelectedGeosetsEach��5,      $��Selected Geosets - Each��65    d� 	IsChecked$
 TrueY�6W   $��0,0,0,5q�6h   	� 5-      -	   5        $��SelectedGeosetsTogether��5-      $!��Selected Geosets - Together��69   5.      �� 50   
   -
   5        $��2��50      $��2��60   $��OK��6=   $��Center=�6J   $��Center=�6g   $��5q�6�   $	��100��6�   51      52      