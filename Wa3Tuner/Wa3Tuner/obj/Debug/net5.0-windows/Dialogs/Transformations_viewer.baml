   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	   %    Wa3Tuner.Transformations_viewer   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5	   	    � Title$ Transformations viewer��5
   	   $	��450��6(   $	��800��65   .��5      �  � RowDefinitions 5   
   �� 5      $��20 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� -   5         ��Name$ TextBlockColumn1��5      $�� ��6.   $��0��66   $��0��6C   $��Center=�6S   $��Center=�6n   �� 5   
   -   5        $ TextBlockColumn2��5      $�� ��6.   $��0��66   $��1��6C   $��Center=�6S   $��Center=�6n   �� 5   
   -   5        $ TextBlockColumn3��5      $�� ��6.   $��0��66   $��2��6C   $��Center=�6S   $��Center=�6n   �� 5   
   -   5         ��Name$ TextBoxColumn1��5       �� 
IsReadOnly$
 True��6*   $��1��6<   $��0��6I    �� AcceptsReturn$
 True��6Y    �� TextWrapping$
 Wrap=�6n   "	 �� VerticalScrollBarVisibility$
	 Auto=�6�   �� 5    
   -   5        $ TextBoxColumn2��5       $
 True��6*   $��1��6=   $��1��6J   $
 True��6Z   $
 Wrap=�6o   $
	 Auto=�6�   �� 5!   
   -   5        $ TextBoxColumn3��5!      $
 True��6*   $��1��6=   $��2��6J   $
 True��6Z   $
 Wrap=�6o   $
	 Auto=�6�   5"      5#      