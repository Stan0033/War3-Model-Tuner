   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   ,    &Wa3Tuner.Dialogs.CreateSkeleton_window   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   
�������  � Title$  Create or load skeleton��5	   	   $	��450��6)   $	��800��66   .��5
      �  � ColumnDefinitions 5   
   �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      .+�5   
   �� $��Create��6   $��Center=�6"   �� 5   
   $
��Load��6   $��1��6    $��Center=�60   �� 5   
   $��Preview��6   $��2��6#   $��Center=�63   �� 5   
   $��1��6   .+�5      �� -   5        $��Format?��5   (   �� 5      -   5        $��InputCreate��5      $	��200��6)    �� AcceptsReturn$
 True��68    �� TextWrapping$
 Wrap=�5      " �� VerticalScrollBarVisibility$
 Auto=�5      �� 5      $��Name:��6   �� 5      -   5        $��InputCreateName��5      $��5q�6-   $��5q�69   �� 5      -   5        $��Create��5   $   $��5q�65   5       �� 5!   
   $��1��6   $��1��6%   .+�5"      �� -   5        $��
ListLoaded��5"      $	��200��6(    � ContextMenu 5#      �� 5$      .@�5%      m� -   5        $��Delete��5%   #   63   5&      5(      �� 5)      -   5        $��Create��5)   "   $��10q�63   5*      R� 5+   
   -   5        $��tree_preview��5+      $��2��6'   $��1��67   5,      5-      