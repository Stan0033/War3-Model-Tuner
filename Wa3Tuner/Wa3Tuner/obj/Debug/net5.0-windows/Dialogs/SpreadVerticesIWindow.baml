   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   ,    &Wa3Tuner.Dialogs.SpreadVerticesIWindow   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5          � 
ResizeMode$  NoResize=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	   
������� � Title$! Spread overlapping vertices��5   	   $	��150��6-   $	��400��6:   .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� $��	Threshold��6   �� 5   
   -   5        $��inputT��5      $��1��6    $��0.001��60   �� 5   
   $ ��Set Distance from centroid��6   $��1��66   �� 5   
   -   5        $��inputD��5      $��1��6    $��1��60   $��1��6=   �� 5   
   -   5        $��2��5      $��2��6)   .��6=   OK6A   5      5      