   M S B A M L   `   `   `  ����C  ?Wa3Tuner, Version=1.3.2.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Wa3TunerWa3Tuner  5   	       Wa3Tuner.createnode   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Wa3Tuner     5   	   -   5        
�������  � 
ResizeMode$  NoResize=�5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � Title$ Create Node��5   	   $	��350��6   $	��400��6*   .��5      � $��10q�6    � ColumnDefinitions 5   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      �� 5      $��20 �6   �� 5      $��50 �6   .+�5   
   �� $
��Name��6   $��Center=�6    $��Center=�6;   �� 5   
   -   5        $	��box��5      $��0��6   $��1��6*   �� 5   
   -   5        $��3��5      $��2��6)   $��10q�6=   $��5q�6I   .��6U   OK6Y   �� 5   
   $
��Type��6   $��1��6    $��Center=�6-   $��Center=�6H   �� 5   
   -   5        $��Check_parent��5      $��Place under selected node��6'   $��2��6K   �� 5   
   -   5        $��	List_Type��5      $��1��6$   $��1��61    �� SelectedIndex$ 0��6A    �� SelectionMode$ Single=�6S   @�5      �� 5       � ToolTip$� �Vertices should only be attached to bones! Bones deform vertices when their transformatons change, creating animation in sequences��6   .��6�   Bone6�   �� 5       $D >Use to hold other nodes. Does no contain additional properties��6   .��6g   Helper6o   �� 5!      $J DUsed for determining in-game how selection insteracts with the model��6   .��6m   Collision Shape6~   �� 5"      $ Illuminates surroundings��6   .��6A   Light6H   �� 5#      .��6   
Emitter16(   �� 5$      $9 3The most used spcial effect. Ex.: Pixies, dust, etc��6   .��6\   
Emitter26f   �� 5%      $R LSpecial effect most visible when the node is moved or the characted is moved��6   .��6u   Ribbon6}   �� 5&      $; 5Used for attaching other models to this model in-game��6   .��6^   
Attachment6j   �� 5'      $O IProduces sound or sprites or special effects in certain sequences in-game��6   .��6r   Event Object6�   5)      5*      5+      