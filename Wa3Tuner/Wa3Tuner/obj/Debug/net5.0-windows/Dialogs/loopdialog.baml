   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.loopdialog   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$
 Loop��5   	   $	��230��6   $	��300��6#   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      �� 5      .+�5   
   ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��In sequence��6   $	��100��6+   �� 5      -   5        $��InputSequence��5      $	��150��6,   5      �� 5   
   $ 
Horizontal=�6   $��1��6.   .+�5      �� $��Times��6   $	��100��6%   �� 5      -   5        $��
InputTimes��5      $	��150��6(   5      �� 5   
   $ 
Horizontal=�6   $��2��6.   .+�5      �� $��Value 1��6   $	��100��6'   �� 5      -   5        $��InputValue1��5      $	��150��6)   5       �� 5!   
   $ 
Horizontal=�6   $��3��6.   .+�5"      �� $��Value 2��6   $	��100��6'   �� 5#      -   5        $��InputValue2��5#      $	��150��6)   5$      �� 5%   
   -   5        $��4��5%      $��OK��6)   5&      5'      