   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.2.7.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.createnode   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � Title$ Create Node��5   	   $	��300��6   $	��400��6*   .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      �� 5      $��20 �6   �� 5      $��40 �6   .+�5   
   �� $
��Name��6   $��Center=�6    $��Center=�6;   �� 5   
   -   5        $	��box��5      $��0��6   $��1��6*   �� 5   
   -   5        $��3��5      $��2��6)   $��10q�6=   .��6I   OK6M   �� 5   
   $
��Type��6   $��1��6    $��Center=�6-   $��Center=�6H   �� 5   
   -   5        $��Check_parent��5      $��Place under selected node��6'   $��2��6K   �� 5   
   -   5        $��	List_Type��5      $��1��6$   $��1��61    �� SelectedIndex$ 0��6A    �� SelectionMode$ Single=�6S   @�5      �� 5      .��6   Bone6$   �� 5       .��6   Helper6&   �� 5!      .��6   Collision Shape6/   �� 5"      .��6   Light6%   �� 5#      .��6   
Emitter16(   �� 5$      .��6   
Emitter26(   �� 5%      .��6   Ribbon6&   �� 5&      .��6   
Attachment6*   �� 5'      .��6   Event Object6,   5)      5*      5+      