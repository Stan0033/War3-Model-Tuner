   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   '    !Wa3Tuner.Dialogs.CameraController   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5          � Title$  Camera Controller��5	   	   $	��200��6%   $	��400��62    � WindowStartupLocation$ CenterScreen=�5
   	    � 
ResizeMode$ NoResize=�5   	   
�������.��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   �� $��Camera Position (Eye):��6   $��0��62   $��0��6?   $��Center=�6O   �� 5   
   -   5         ��Name$ CameraPositionX��5      $��0��6+   $��1��68   $��5q�6H   �� 5   
   -   5        $ CameraPositionY��5      $��0��6+   $��2��68   $��5q�6H   �� 5    
   -   5        $ CameraPositionZ��5       $��0��6+   $��3��68   $��5q�6H   �� 5"   
   $ ��Target Position (Look at):��6   $��1��66   $��0��6C   $��Center=�6S   �� 5#   
   -   5        $ TargetPositionX��5#      $��1��6+   $��1��68   $��5q�6H   �� 5$   
   -   5        $ TargetPositionY��5$      $��1��6+   $��2��68   $��5q�6H   �� 5%   
   -   5        $ TargetPositionZ��5%      $��1��6+   $��3��68   $��5q�6H   �� 5'   
   $��Roll:��6   $��2��6!   $��0��6.   $��Center=�6>   �� 5(   
   -   5        $ RollX��5(      $��2��6!   $��1��6.   $��5q�6>   �� 5)   
   -	   5        $ RollY��5)      $��2��6!   $��2��6.   $��5q�6>   �� 5*   
   -
   5        $ RollZ��5*      $��2��6!   $��3��6.   $��5q�6>   �� 5,   
   -   5        $��OK��5,      $��3��6)   $��4��66   $��Center=�6J   $��5q�6g   $��75��6r   5-      5.      