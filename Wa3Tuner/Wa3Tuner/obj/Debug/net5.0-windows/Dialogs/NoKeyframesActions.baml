   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Wa3Tuner.DialogsWa3Tuner.Dialogs  5   	   )    #Wa3Tuner.Dialogs.NoKeyframesActions   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:Wa3Tuner.Dialogs     5   	   -   5        
�������  � Title$  Node's keyframes action��5
   	   $	��340��6)   $	��500��66   .��5      �  � ColumnDefinitions 5   
   �� 5      �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��90 �6   �� 5      �� 5      $��30 �6   .+�5   
   �� $��10q�6   .+�5      �� $��Transformation:��6   �� 5      -   5        $��check_1��5      .��6&   Translation63   �� 5      -   5        $��check_2��5      .��6&   
Rotation60   �� 5      -   5        $��check_3��5      .��6&   	Scaling6/   5      �� 5   
   $��10q�6   $��1��6!   .+�5      �� $��Action Type:��6   	� 5      -   5        $��action_1��5       d� 	IsChecked$
 TrueY�6*   .��6;   Copy6A   	� 5      -   5        $��action_2��5      .��6*   
Cut/Move64   5!      �� 5"   
   $��10q�6   $��2��6!   $��2��61   .+�5#      �� .+�5$      �� $��Paste method:��6   	� 5%      -   5        $��method_1��5%      $
 TrueY�6.   .��6?   Don't Resize6M   	� 5&      -   5        $��method_2��5&      .��6.   Resize if needed6@   5'      �� 5(      .+�5*      	� -	   5        $��method_3��5*      $
 TrueY�6.   .��6?   Overwrite all6N   	� 5+      -
   5        $��method_4��5+      .��6.   Overwrite duplicates6D   	� 5,      -   5        $��method_5��5,      .��6.   Skip duplicates6?   	� 5-      -   5        $��method_6��5-      .��6.   Only duplicates6?   5.      50      �� 51   
   $��1��6   $��10q�6"   .+�52      �� $��From sequence��6   �� 53      -   5        $��list1��53   #   $	��150��6   62   54      �� 56   
   $��1��6   $��1��6"   $��10q�62   .+�57      �� $��To sequence��6   �� 58      -   5        $��list2��58   #   $	��150��6   62   5:      �� 5;   
   -   5        $��2��5;      $��1��6)   $��30��6;   .��6G   Apply6N   5=      5>      