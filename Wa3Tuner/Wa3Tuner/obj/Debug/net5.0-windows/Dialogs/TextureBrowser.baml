   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.1.7.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.TextureBrowser   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	   
������� � Title$ Texture Finder��5   	   $	��450��6    $	��400��6-    � 	Resources 5       �� 
TargetType �� Property �� Value��%_  '��       '�� �     �� 5   
   " ��6   .�5      ��  � ��$ 30��6'   �� 5       � ��$ 5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��$ Stretch=�64   5      �� 5   
   " ��6   .�5      ��  � ��$ 35��6'   �� 5       � ��$ 5q�6'   �� 5       � ��$ Stretch=�64   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5       $��* �6   �� 5"      $��* �6   �� 5$      $
��Auto �6    � ColumnDefinitions 5(   
   �� 5)      $��* �6   .+�5,   
   �� -   5        $��	SearchBox��5,      $��0��6#   60   �� 5.   
   -   5        $��ItemListBox��5.      $��1��6%    �� SelectionMode$ Single=�6R   �� 50   
   -   5        $��ImageHolder��50      $��2��6#   �� 52   
   $��3��6   	 �� Orientation$	 
Horizontal=�6"   $��Stretch=�6;   .+�53      �� -   5        $��AddTextureButton��53      $��Add texture��6@   $	��100��6V   �� 54      -   5        $!��AddTextureAndMaterialButton��54      $��Add Texture and Material��6N   $	��200��6q   55      56      57      