   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.0.8.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.window_edit_cols   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   
�������  � WindowStartupLocation$  CenterScreen=�5	   	    � 
ResizeMode$ NoResize=�5
   	    � Title$ Edit collision shape data��5   	   $	��250��6,   $	��400��69   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��80 �6   �� 5      $
��Auto �6   �� 5      $��80 �6   �� 5      $
��Auto �6   �� 5      $��80 �6   .+�5"   
   �� $��Type:��6   $��Center=�6!   $��Center=�6>   	� 5#   
   -   5         	�Name$ BoxRadioButton��5#      $	��Box��6.   $��0��6<   $��1��6I   $��2��6Y   $��5q�6m   6x   	� 5$   
   -   5        $ SphereRadioButton��5$      $��Sphere��61   $��1��6B   $��1��6O   $��2��6_   $��5q�6s   6~   �� 5'   
   $��Box Extents��6   $��2��6'   $��0��64   $��6��6D   $��5q�6X   �� 5*   
   $��Negative Extent X:��6   $��3��6.   $��0��6;   $��Center=�6K   $��5q�6f   �� 5+   
   -   5         ��Name$ NegativeExtentXTextBox��5+      $��3��62   $��1��6?   $��60��6O   $��5q�6Z   �� 5.   
   $��Y:��6   $��3��6   $��2��6+   $��Center=�6;   $��5q�6V   �� 5/   
   -   5        $ NegativeExtentYTextBox��5/      $��3��62   $��3��6?   $��60��6O   $��5q�6Z   �� 52   
   $��Z:��6   $��3��6   $��4��6+   $��Center=�6;   $��5q�6V   �� 53   
   -   5        $ NegativeExtentZTextBox��53      $��3��62   $��5��6?   $��60��6O   $��5q�6Z   �� 56   
   $��Positive Extent X:��6   $��4��6.   $��0��6;   $��Center=�6K   $��5q�6f   �� 57   
   -   5        $ PositiveExtentXTextBox��57      $��4��62   $��1��6?   $��60��6O   $��5q�6Z   �� 5:   
   $��Y:��6   $��4��6   $��2��6+   $��Center=�6;   $��5q�6V   �� 5;   
   -   5        $ PositiveExtentYTextBox��5;      $��4��62   $��3��6?   $��60��6O   $��5q�6Z   �� 5>   
   $��Z:��6   $��4��6   $��4��6+   $��Center=�6;   $��5q�6V   �� 5?   
   -   5        $ PositiveExtentZTextBox��5?      $��4��62   $��5��6?   $��60��6O   $��5q�6Z   �� 5B   
   $��Sphere Radius��6   $��5��6)   $��0��66   $��6��6F   $��5q�6Z   �� 5C   
   $��Radius:��6   $��6��6#   $��0��60   $��Center=�6@   $��5q�6[   �� 5D   
   -	   5        $ RadiusTextBox��5D      $��6��6)   $��1��66   $��80��6F   $��5q�6Q   �� 5G   
   -
   5        $��OK��5G      $��7��6   $��70��6+   $��5��6A   $��6��6Q   $��Center=�6e   $��5q�6�   5H      5I      