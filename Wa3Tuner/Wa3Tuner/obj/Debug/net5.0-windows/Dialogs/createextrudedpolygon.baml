   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	   "    Wa3Tuner.CreatePolygonWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5          � 
ResizeMode$  NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ Create Extruded Polygon��5   	   $	��500��6)   $	��800��66   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��542 �6   �� 5      $��58 �6   �� 5      $	��200 �6   .+�5   
   �� -   5        $��ImageHolder��5      $��0��6'   $��0��67   
������ $��2��6`   .+�5      �� -   5        $��ImageComparison��5      $	��450��6,   $	��600��69   5      �� 5   
   -   5        $��
MainCanvas��5      $��0��6%   $��0��65   
������ $��2��6[   �� 5   
   -   5        $��SelectionLayer��5      $��0��6*   $��0��6:   
������ $��2��5   |   �� 5   
   $��2��6    �� Orientation$ Vertical=�6%   $��10,10,10,10q�6<   
�������.+�5      �� -   5        $��ModeDisplay��5      $��Work Mode: None��6+   $��5q�6B   �� 5       -   5        $��FirstTextBlock��5       $��Number of vertices:��6.   $��5q�6I   �� 5!      -   5        $��InputTextBox��5!      $	��100��6*   $��5q�66   6C   �� 5"      $��	(3 - 100)��6   $��Center=�6)   �� 5#      $��LMB for selection��6   $��5q�61   �� 5$      $��RMB to move��6   $��5q�6,   �� 5%      $'��!_________________________________��6   $��5q�6B   �� 5&      -	   5        $��CheckExtruded��5&      .��6`   
Extruded6j   �� 5'      $ 
Horizontal=�6   .+�5(      �� $��
Extrusion:��6   $��5q�6.   �� 5)      -
   5        $��ExtrusionInput��5)      $	��100��60   $��5q�6<   $��2��6G   5*      �� 5,      -   5        $��Import��5,      $	��100��66   $��2q�6B   �� 5-      -   5        $��Export��5-      $	��100��66   $��2q�6B   �� 5.      -   5        $��Load image...��5.      $	��100��6@   $��0,30,0,0q�6L   �� 50      $��	10,50,0,0q�6   .+�51      �� -   5        $��CheckRotate��51      $��5q�6/   .��6;   Rotate Toward Camera6Q   �� 52      -   5        $��Finish��52      $	��100��6+   67   53      55      56      57      