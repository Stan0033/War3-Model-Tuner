   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   -    'Wa3Tuner.Dialogs.Gradual_Keyframe_Maker   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ Gradual Keyframe Maker��5   	   $	��450��6(   $	��800��65   .��5      �  � ColumnDefinitions 5   
   �� 5      $	��37* �6   �� 5      $
��163* �6   �� 5      $
��200* �6    � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      �� 5      $��30 �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��5,5,5,5q�6.   $��2��6?   .+�5      	� -   5        $��c1��5   +    d� 	IsChecked$
 TrueY�6   $	��170��6E   .��6Q   With number of keyframes6k   	� 5      -   5        $��c2��5      .��65   	With step6@   5      �� 5   
   -   5        $
��list��5      $��1��6   $��2��6T   5      �� 5    
   $��1��6   $��2��6"   .+�5!      �� $ 
Horizontal=�6   .+�5"      �� $��From:��6   $	��150��6)   �� 5#      -   5        $��	inputFrom��5#      $	��100��6+   5$      �� 5%      $ 
Horizontal=�6   .+�5&      �� $	��To:��6   $	��150��6'   �� 5'      -   5        $��inputTo��5'      $	��100��6)   5(      �� 5)      $ 
Horizontal=�6   .+�5*      �� $��Initial value:��6   $	��150��62   �� 5+      -   5        $��
inputValue��5+      $	��100��6,   5-      �� 5.      -   5        $��d1��5.   2   $ 
Horizontal=�6   .+�5/      �� $ ��Number of keyframes total:��6   $	��150��6>   �� 50      -	   5        $��inputKF��50      $	��100��6)   $��2��65   51      �� 52      -
   5        $��d2��52   2   $ 
Horizontal=�6   .+�53      �� $��Step:��6   $	��150��6)   �� 54      -   5        $��	inputStep��54      $	��100��6+   $��1��67   �� 55      -   5        $��checkIncrement��55      $
 TrueY�61   .��6B   	Increment6M   56      �� 57      -   5        $��20q�57       $��40��6,   .��68   OK6<   58      59      5:      