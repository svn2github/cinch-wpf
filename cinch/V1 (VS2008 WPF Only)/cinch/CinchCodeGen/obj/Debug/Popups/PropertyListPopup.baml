   M S B A M L   `   `   `  ����O  Kmscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      G CCinchCodeGen, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:CinchCodeGenCinchCodeGen 5      $   CinchCodeGen.PropertyListPopup   5      W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     8x,http://schemas.microsoft.com/winfx/2006/xaml    5      4sys&clr-namespace:System;assembly=mscorlib    5      *localclr-namespace:CinchCodeGen    5      -   5          �Name$	  win��5       � Title$ Cinch MVVM Code Generator��5       � Topmost .   � WindowStartupLocation$ CenterOwner=�5       � WindowState$ Normal=�5      $	��400��5      $	��600��5      $	��400��5      $	��600��5       � WindowStyle$
 None=�5         WindowTemplateKey#��[  5       � 
ResizeMode$ CanResizeWithGrip=�5       � AllowsTransparency . ��5	      Z� .��5      ��  �� CornerRadius$ 10��6   ���	#ff656565$��0q�66   ._�5   
   � 
������ $��2q�6(   .+�5      �� $ 10,10,10,10��6   
�������
���   �$��5q�5   )   ._�5      � ���	#ff656565	 � RowDefinitions	 5      �� 5      $��50 �6(   �� 5       $��* �6(   �� 5!      $
��Auto �6(   .+�5%      � -   5        $��0��5%      $��Stretch=�6(   $��50��6F   $��0,0,0,0q�6R   
���   �.+�5(      �� $��../Images/header.png��6    $	��250��6>   $��25��6J   $��10,5,5,5q�6V   $
��Left=�5)      $��Center=�67   5*      � 5-      $��1��6   $��Stretch=�6(   $��Stretch=�5.      
 � ColumnDefinitions
 5/      �� 50      $��* �6/   �� 51      $
��Auto �6/   	 54      �� 55      $
��Auto �6,   �� 56      $��* �6,   .+�59      �� $��Arial)�6    $��13(�63   $��0��6A   $��0��6N   $��2��5:      $+��%Add / Remove Available Property Types��5;      $��5q�6L   $��Center=�5<      
���   �$��Italic$�5=      $
��Bold"�6/   $
��Left=�5>      �� 5@      
���   �$��1��64   $��0��6A   $��1��6Q   $��
10,0,10,10q�5A   !   $��Stretch=�5B   !   $��Stretch=�6?   $ 5��6[   ._�5D      �� -   5         ��Name$	 lst��5D   &   
������ 
������ $��5q�5E   Q   $ �� IsSynchronizedWithCurrentItem$
 TrueY�5F        PropertyItemStyle#���  5G       �� AlternationCount$ 2��5I      $��Stretch=�5J      $��Stretch=�6<   ��5H      �� *PropertyTypes+ � 	Resources 5K   "    �� 
TargetType �� Triggers P� Property P� Value �� Property �� Value��%k  &	       �� 5L   &   " ��6F    5M   *   P� 5N   .    �@��AlternationIndex$ 0��6_   . �5O   2   ��  � ��
 �����6O   5P   /   P� 5Q   .    �@��AlternationIndex$ 1��6_   . �5R   2   ��  � �� �	#ff8080806O   5S   /   P� 5T   .    �@J�IsMouseOver . . �5U   2   ��  � �� �	#FFEF68036O   5V   /   5X   '   5Y   #   5Z      5[      �� 5]      
���   �
���   �$��1q�6H   $ 5��5^      $��0��6.   $��1��6;   $��1��6K   $��2��5_      $��10,35,10,10q�6.   ._�5`      ��  �� Orientation$ Vertical=�6)   .+�5b   "   �� 
 � Tag$ ../Images/Add.png��6)   $��30��5c   !   $��30��6,   $��Center=�69     imageButtons#��[ 5d   !   $��2,4,2,4q�6G    � ToolTip$ Add new property��5e   !   ��5f   !   �� *AddNewPropertyTypeCommand+�� 5h   "   $ ../Images/error.ico��6)   $��25��5i   !   $��25��6,   $��Center=�69   #��[ 5j   !   $��2,4,2,4q�6G   $ Delete current property��5k   !   ��5l   !   �� *RemovePropertyTypeCommand+5n      5o      5q      �� 5t      $��2��6!   $��Stretch=�6.   $��Stretch=�5u      $ 
Horizontal=�63   .+�5w      �� $ ../Images/ok.png��6!    �� 	IsDefault . $��30��5x      $��30��6(   $
��Left=�65   #��[ 5y      $��20,10,10,10q�6C   $5 /Ok : Accept the current available property list��5z      $
��True��5|      ��5{      �� *CloseActivePopUpCommand+5|      �� 5~      $ ../Images/cancel.png��6!    �� IsCancel . $��30��5      $��30��6(   $
��Left=�65   #��[ 5�      $��20,10,10,10q�6C   $C =Cancel : Dismiss edits to the current available property list��5�      5�      5�      5�      �� 5�      $ 
Horizontal=�6   $��Right=�62   $��	0,10,20,0q�6N   $	��Top=�5�   %   $
��Auto��6=   $
��Auto��6K   .+�5�      �� -   5         ��Name$ btnClose��5�      $��	Wingdings)�6+   $��û��6B    J� RenderTransformOrigin$ 0.5,0.5&�5�      $��35,0,0,0q�65     mainWinButtons#��[ 5�      $ Close��5�      z�5�      V� 5�      .~�5�      ��  �� ScaleX$	 0.7Q�6-    �� ScaleY$	 0.7Q�6:   �� 5�       �� AngleX$ 0Q�6,    �� AngleY$ 0Q�67   �� 5�        �� Angle$  -13.184Q�6.   T� 5�      ! T� X$! 0Q�61   " T� Y$" 0Q�67   5�      5�      5�      5�      5�      5�      