   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   2    ,Wa3Tuner.Dialogs.SequenceToSequencesSelector   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   
�������  � WindowStartupLocation$  CenterScreen=�5
   	    � Title$$ Sequence to sequences selector��5   	   $	��250��60   $	��450��6=   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      �� 5      $��30 �6   .+�5   
   �� $��From sequence:��6   �� 5   
   -   5        $��list1��5       �� SelectionMode$ Single=�6   $��1��66   �� 5   
   $��To sequence/s:��6   $��1��6*   �� 5   
   -   5        $��list2��5      $ Multiple=�6   $��1��68   $��1��6E   �� 5   
   -   5        $��ButtonOk��5      $��2��6J   $��1��6W   .��6g   OK6k   5      5      