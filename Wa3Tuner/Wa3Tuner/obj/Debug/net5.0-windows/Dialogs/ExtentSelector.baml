   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   %    Wa3Tuner.Dialogs.ExtentSelector   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$! Extent Positioning Selector��5   	   $	��450��6-   $	��400��6:   .��5      �  � RowDefinitions 5   
   �� 5      �� 5      �� 5      $��30 �6    � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   �� $��5q�6   $��2��6    .+�5      	� -   5        $��m1��5       d� 	IsChecked$
 TrueY�6$   .��65   Model's Extent6E   	� 5      -   5        $��m2��5      .��6$   Geoset Extent63   	� 5      -   5        $��m3��5      .��6$   Geoset Sequence Extent6<   	� 5      -   5        $��m4��5      .��6$   Sequence Extent65   	� 5      -   5        $��m5��5      .��6$   %#Extent of Attached vertices to node6I   5      �� 5   
   -   5        $
��list��5      $��1��6   6+   �� 5   
   -   5        $��list2��5      $��1��6   $��1��6,   �� 5    
   -	   5        $��2��5       $��2��6*   .��6>   OK6C   5!      5"      