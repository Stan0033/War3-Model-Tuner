   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   +    %Wa3Tuner.Dialogs.Model_Colors_Changer   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5
   	    � Title$< 6Model Colors Changer - Create variations of your model��5   	   $	��150��6H   $	��600��6U   .��5      � $��10q�6   
������� � ColumnDefinitions 5   
   �� 5      $
��114* �6   �� 5      $	��31* �6   �� 5      $
��145* �6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      .+�5   
   �� $��Color:��6   $��2��6"   �� 5   
   -   5        $��Combo��5      $��2��6    62   �� 5   
    �� Orientation$ 
Horizontal=�6   $��1��6.   $��3��6;   .+�5      �� -   5        $��c1��5      $��Static GeosetAnim Colors��6!   �� 5      -   5        $��c2��5      $ ��Animated GeosetAnim Colors��6!   $��2q�6F   �� 5      -   5        $��c3��5      $��TC��6!   $��2q�6.   �� 5      -   5        $��c4��5      $��TGlow��6!   $��2q�61   �� 5      -   5        $��c5��5      $��Stars��6!   $��2q�61   �� 5      -   5        $��c6��5      $��Glows��6!   $��2q�61   5       �� 5!   
   -	   5        $
��SEEK��5!      $9��3Seek to change GAs based on
 closest color to color��6   $��2q�6b   $��2��6m   �� 5"   
   -
   5        $��2��5"      $��2��6*   $��5,5,5,5q�6:   $��OK��6K   5#      5$      