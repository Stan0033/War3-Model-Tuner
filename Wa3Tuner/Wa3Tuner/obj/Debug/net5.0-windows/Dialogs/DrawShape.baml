   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.DrawShapeWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	     � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5	   	    � Title$
 Draw��5
   	   $	��550��6   $	��800��6#   .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��200 �6   .+�5   
   �� -   5        $��Canvas_Draw��5      $��0��6$   
�������5      �� 5   
   $��1��6   
�������.+�5      �� $�� ��6   $��Center=�6    $��10q�6=   �� 5      -   5        $��ButtonPencil��5      $��Pencil��6)   $��30��6:   $	��150��6F   
����� �6i   �� 5      -   5        $��
ButtonLine��5      $
��Line��6'   $��30��66   $	��150��6B   6N   �� 5      -   5        $��
ButtonEdit��5      $
��Edit��6'   $��30��66   $	��150��6B   6N   �� 5!      -   5        $
��Undo��5!      $��30��6$   $	��150��60   $��0,20,0,0q�6J   �� 5"      -   5        $
��Redo��5"      $��30��6$   $	��150��60   6<   �� 5#      -   5        $��Clear history��5#      $��30��6-   $	��150��69   6E   �� 5$      -   5        $��Clear��5$      $��30��6%   $	��150��61   6=   �� 5&      -	   5        $��Finish��5&      $��30��6&   $	��150��62   $��0,50,0,0q�6>   6P   �� 5'      $��Extrude amount��6   �� 5(      -
   5        $��InputExtrude��5(      $��5q�6*   $��2��65   �� 5*      $��	Face axes��6   	� 5+      -   5        $��check_x��5+      $��X��6)   $��2q�65    d� 	IsChecked$
 TrueY�6@   	� 5,      -   5        $��check_y��5,      $��Y��6)   $��2q�65   	� 5-      -   5        $��check_z��5-      $��Z��6)   $��2q�65   5.      5/      50      