   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.color_selector   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � Title$ Color Selector��5   	   $	��220��6    $	��300��6-   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��50 �6   �� 5      $��* �6   �� 5      $��40 �6   .+�5   
   �� -   5        $��DisplayR��5      $��0��6$   $��2��6-   �� 5   
   -   5        $��DisplayG��5      $��0��6$   $��2��6-   $��1��6=   �� 5   
   -   5        $��DisplayB��5      $��0��6$   $��2��6-   $��2��6=   �� 5   
   $	��Red��6   $��Center=�6   �� 5   
   -   5         ��Name$ 	RedSlider��5       � Minimum$ 0Q�6$    � Maximum$	 255Q�60    � Value$	 128Q�6>   $��1��6J   6Z   �� 5!   
   $��Green��6   $��1��6!   $��Center=�6.   �� 5"   
   -   5        $ GreenSlider��5"      $ 0Q�6&   $	 255Q�62   $	 128Q�6@   $��1��6L   $��1��6\   6i   �� 5$   
   $
��Blue��6   $��2��6    $��Center=�6-   �� 5%   
   -   5        $ 
BlueSlider��5%      $ 0Q�6%   $	 255Q�61   $	 128Q�6?   $��1��6K   $��2��6[   6h   �� 5'   
   -   5        	 ��Name$	 ColorCanvas��5'      $��3��6&   $��2��63   $��50��6G   
�������$��	0,10,0,10q�6e   �� 5)   
   -	   5        
 ��Name$
 OkButton��5)      $��OK��6#   $��4��60   $��2��6=   $��75��6Q   $��Center=�6\   6y   5*      5+      