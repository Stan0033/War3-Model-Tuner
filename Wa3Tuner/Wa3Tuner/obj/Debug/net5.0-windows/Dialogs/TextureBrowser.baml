   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.TextureBrowser   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	   
������� � Title$ Texture Finder��5   	   $	��450��6    $	��500��6-    � 	Resources 5       �� 
TargetType �� Property �� Value��%_  '��       '�� �     �� 5   
   " ��6   .�5      ��  � ��$ 30��6'   �� 5       � ��$ 5q�6(   �� 5       � ��$ 5q�6'   �� 5       � ��$ Stretch=�64   5      �� 5   
   " ��6   .�5      ��  � ��$ 35��6'   �� 5       � ��$ 5q�6'   �� 5       � ��$ Stretch=�64   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6   �� 5       $
��Auto �6   .+�5%   
   �� -   5        $
��Tabs��5%      $��0��6!   .@�5&      �� $
��Find��6   .��5'      �  � ColumnDefinitions 5(      �� 5)      �� 5*      $	��37* �6+    5,      �� 5-      $
��Auto �6(   �� 5.      $��* �6(   .+�50      �� -   5        $��FindSearchBox��50      $��0��63   $��2��6`   $��5,5,5,5q�6t   �� 51      -   5        $��FindItemListBox��51      $��1��65    �� SelectionMode$ Single=�52      $��2��65   	 � ContextMenu	 53      �� 54      .@�55   "   m� -   5        $��Export��55   +   6;   m� 56   "   -   5        $��Export as PNG��56   +   6B   m� 57   "   -   5        $��Copy path to clipboard��57   +   6K   58      5:      5;      5<      �� 5=      $��
Favourites��6   .��5>      �  5?      �� 5@      $	��42* �6+   �� 5A      $
��205* �6+    5C      �� 5D      $
��Auto �6(   �� 5E      $��* �6(   .+�5G      �� -   5        $��FavSearchBox��5G      $��0��62   $��2��6^   $��5,5,5,5q�6r   �� 5H      -	   5        $��FavItemListBox��5H      $��1��64   $ Single=�6a   $��2��6x   5I      5J      5K      �� 5N   
   -
   5        $��ImageHolder��5N      $��1��6#   �� 5Q   
   $��2��6   
 �� Orientation$
 
Horizontal=�6"   $��Stretch=�6;   .+�5R      �� -   5        $��AddTextureButton��5R      $��Add texture��6@   $	��100��6V   �� 5S      -   5        $!��AddTextureAndMaterialButton��5S      $��Add Texture and Material��6N   $	��200��6q   �� 5T      -   5        $��ButtonAddFavourites��5T      $��Add Favourite��6E   $	��150��6]   �� 5U      -   5        $��DelFavouriteButton��5U      $	��Del��6D   $	��150��6R   $y�	Collapsed=�6^   5V      5W      5X      