   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   &     Wa3Tuner.Dialogs.KeyframesCopier   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   
�������  � Title$  Copy/Move keyframes range��5	   	   $	��450��6+   $	��800��68   .��5
      �  � RowDefinitions 5   
   �� 5      $��70 �6   �� 5      �� 5      �� 5      .+�5   
   �� $��10q�6   .+�5      �� $��Action��6   	� 5       d� 	IsChecked$
 TrueY�6   .��6+   Copy61   	� 5      .��6   
Cut/Move6$   5      �� 5   
   $��10q�6   $��1��6!   .+�5      �� $��Copy Method��6   	� 5      $
 TrueY�6   .��6+   From range to range6@   	� 5      .��6   From range to time6/   	� 5      .��6   From range to sequence63   	� 5      .��6   From sequence to time62   	� 5      .��6   From sequence to range63   	� 5      .��6   From sequence to sequence66   5      �� 5    
   $��10q�6   $��2��6!   .+�5!      �� $��Paste Method��6   	� 5"      $
 TrueY�6   .��6+   Overwrte all69   	� 5#      .��6   Only paste duplcate times66   	� 5$      .��6   Skip duplicate times61   5'      �� 5(   
   $��10q�6   $��3��6!   .+�5)      �� $��Stretch?��6   	� 5*      $
 TrueY�6   .��6+   Dont Stretch69   	� 5+      .��6   	Stretch6$   5/      50      51      