   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   -    'Wa3Tuner.Dialogs.geoVisibilitiesKeyword   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	     � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5	   	   
������� � Title$/ )Edit visibilities based on keyword/s (OR)��5   	   $	��150��6;   $	��400��6H   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      $��30 �6   .+�5   
   	� -   5        $��r1��5       d� 	IsChecked$
 TrueY�6    .��61   Visible only in6B   	� 5   
   -   5        $��r2��5      $��1��6    .��6-   Not visible only in6B   �� 5   
   -   5        $	��box��5      $��1��6   
������� �� TextWrapping$
 Wrap=�6D    �� AcceptsReturn$
 True��6X   .��6m   5      �� 5   
   -   5        $��1��5      $��1��6!   .��69   Finish6A   5      5      