   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   "    Wa3Tuner.Dialogs.NodePresets   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5
   	   
������� � Title$ Node Presets��5   	   $	��250��6   $	��400��6+   $	��400��67   $
��1000��6F   .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   .+�5   
   �� -   5        $
��list��5      
������� � ContextMenu 5      �� 5      .@�5      m� -   5        $��~Delete��5      60   5      5      �� 5   
   -   5        $��input��5      $��1��6   6   �� 5   
   -   5        $��2��5      .��6-   Create65   �� 5   
   -   5        $��check��5      $��3��6    $��Put under selected node��6-   5      5      