   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   '    !Wa3Tuner.Dialogs.Snap_Selector_2d   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5          � 
ResizeMode$  NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5	   	   
������� � Title$ Snap Selector ��5   	   $	��300��6    $	��360��6-   .��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��25 �6   .+�5   
   �� $	��512��6   $	��512��6   $��Center=�6*   $��Center=�6G   .+�5      �� $	��300��6   $	��200��6$   
���   �$��2��6@   
������  �� Left$	 106��6g   
 �� Top$	 156��6y   	� 5      -   5        $	��r_n��5      $��Center=�6%   $��Center=�6B   $��16(�6]   $
��Bold"�6k   
���   �$	 206��5   -   $	 256��6?    d� 	IsChecked$
 TrueY�6P   .��6a   Snap to nearest5      	� 5      -   5        $
��r_tl��5      $��Top Left��6&    	� 	GroupName$ SnapType��69   $	 107��6N   $	 159��6`   $��80��6q   $��20��6|   	� 5      -   5        $
��r_tr��5      $��	Top Right��6&   $ SnapType��6:   $	 330��6O   $	 159��6a   $��80��6r   $��20��6}   	� 5       -   5        $
��r_br��5       $��Bottom Left��6&   $ SnapType��6<   $	 106��6Q   $	 356��6c   $��80��6t   $��20��6   	� 5!      -   5        $
��r_bl��5!      $��Bottom Right��6&   $ SnapType��6=   $	 320��6R   $	 356��6d   $��80��6u   $��20��6�   5"      �� 5#   
   -   5        $��1��5#      .��6)   OK6-   5$      5%      