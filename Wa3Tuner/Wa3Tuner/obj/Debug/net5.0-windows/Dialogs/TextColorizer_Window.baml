   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   +    %Wa3Tuner.Dialogs.TextColorizer_Window   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$, &Text colorizer for warcraft 3 tooltips��5   	   $	��250��68   $	��400��6E   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      $��30 �6   �� 5      $��30 �6   .+�5    
   �� -   5        $��MainTextBox��5       $��0��6+   $��7��6;   ���	#D80E0E0E
�������5!   .   �� 5#   
   -   5        $��ButtonColor1Select��5#   8   $��Color 1��6&   $��1��6R   �� 5$   
   -   5        $	��Set��5$   #   $��1��61   $��1��6?   �� 5%      -   5        $��ButtonColor2Select��5%   =   $��Color 2��6+   $��1��6W   $��2��6e   �� 5&   
   -   5        $��ButtonColor2Set��5&   1   $	��Set��6#   $��1��6H   $��3��6V   �� 5'   
   -   5        $��Copy all��5'   #   $��2��68   $��6��6F   �� 5(   
   -   5        $��Clear��5(   '   $��1��67   $��4��6D   �� 5)   
   -	   5        $��	Clear All��5)   (   $��1��6<   $��5��6I   5*      5+      