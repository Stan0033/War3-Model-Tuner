   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   '    !Wa3Tuner.Dialogs.DistanceSelector   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � Title$  Distancing selector��5
   	   $	��200��6%   $	��400��62   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      $��20 �6   �� 5      .+�5   
   �� $��From Point:��6   $��16(�6'   $��Center=�65   $��Center=�6R   �� 5   
   $��X:��6   $��16(�6   $��1��6,   $��Center=�69   $��Center=�6V   �� 5   
   -   5        $��InputX��5      $��1��6    $��1��60   $��0��6=   �� 5   
   $��Y:��6   $��2��6   $��16(�6+   $��Center=�69   $��Center=�6V   �� 5    
   -   5        $��InputY��5       $��2��6    $��2��6-   $��0��6=   �� 5!   
   $��Z:��6   $��3��6   $��16(�6+   $��Center=�69   $��Center=�6V   �� 5"   
   -   5        $��InputZ��5"      $��3��6    $��1��6-   $��0��6=   �� 5#   
   $��	Distance:��6   $��4��6%   $��16(�62   $��Center=�6@   $��Center=�6]   �� 5$   
   -   5        $��InputDistance��5$      $��4��6'   $��1��64   $��0��6E   	� 5%   
   -   5        $��RadioSet��5%   1   $��5��6   $	��Set��6#    d� 	IsChecked$
 TrueY�6A   	� 5&   
   -   5        $��Radiop��5&   1   $��6��6   $	��Add��6#   61   	� 5'   
   -   5        $��Radiom��5'   6   $��7��6   $��Subtract��6#   66   	� 5(   
   -	   5        $��RadioMul��5(   F   $��5��6   $��1��6#   $��Multiply��63   6F   	� 5)   
   -
   5        $��RadioDiv��5)   D   $��6��6   $��1��6#   $��Divide��63   6D   	� 5*   
   -   5        $��RadioMod��5*   D   $��7��6   $��1��6#   $��Modulo��63   6D   �� 5+   
   -   5        $��8��5+      $��2��6)   .��6=   OK6A   5,      5-      