   M S B A M L   `   `   `  ����@  <Cinch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,"clr-namespace:Cinch;assembly=CinchCinch  5       	MVVM.Demo!   MVVM.Demo.AddEditOrderPopup   5   	   R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      2Cinch"clr-namespace:Cinch;assembly=Cinch    5        � Title$  Customer Orders��5      $	��350��6   $	��500��6*    � Icon$ ../Images/Orders.png��5       � 
ResizeMode$ NoResize=�5       � WindowStartupLocation$ CenterOwner=�5       � 	Resources 5
         ProductTemplate �� Orientation �� Path��%�  &	        �� 5   
   .R�5      �� $ Vertical=�6   .+�5      �� 
���   �$��CharacterEllipsis=�5      $��Stretch=�6:   $��2q�5      $��Center=�6$   ��5      �� $ ProductName�5   $   �� 5      
���   �$��CharacterEllipsis=�5      $��Stretch=�6:   $��2q�5      $��Center=�6$   $
��Bold"�6?   ��5      �� $ ProductPrice�5   ?   5      5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��60 �6   �� 5       $
��Auto �6   �� 5!      $��* �6   .+�5%   
   � $��0��6   $��Stretch=�6   $��Stretch=�6:   
���   � 5(      �� 5)      $��* �6    �� 5*      $��2 �6    .+�5-      �� $��10,0,0,0q�6   $ 
Horizontal=�6+   $��Stretch=�6D   .+�5.      �� $��../Images/Orders.png��6   $
��Left=�66   $��Center=�5/      $��36��6/   $��36��6:   �� 52      $
��Bold"�68   
��� ���$��18(�53      $
��Left=�6"   $��Center=�6=   ��52      �� *DisplayName+53   =   56      �� 58      $��1��6   $��Stretch=�6%   $��2��6C   
���   �$��Bottom=�59      5<      �� 5?   
   $��1��6   
�������
���   �$��2q�5@   '   $��40,40,40,10q�6;   ��5A      �� 5B      $��Order Details��6   $
��Bold"�60   $��14(�5C      $
��Left=�6'   $��Center=�6B   .��5G      �  5H      �� 5I      $
��Auto �6$   �� 5J      $
��Auto �6$   �� 5K      $
��Auto �6$    � ColumnDefinitions 5N      �� 5O      $��* �6'   .+�5T      �� $��0��6   $��5q�6*   $ 
Horizontal=�5U      .+�5V      �� $��Product��6   $��Normal"�6.   $��11(�6B   $	��150��6P   �� 5W      $	��200��6   $��Center=�5Y      $	 �� IsSynchronizedWithCurrentItem$
	 TrueY�5Z      $��50��5[      #��[  5\      
 �� SelectedItem
 5X      �� $ CurrentProduct���5[   %   �� $ Products�5\      5]      �� 5a      $��1��6   $��5q�6*   $ 
Horizontal=�5b      .+�5c      �� $��Quantity��6   $��Normal"�6/   $��11(�6C   $	��150��6Q   �� 5d      $��Normal"�6   $��11(�62   $	��200��6@   "   Cinch.NumericTextBoxBehavior   	IsEnabled .   ValidatingTextBox#��[ 5i      ��5f      �� $- 'CurrentCustomerOrder.Quantity.DataValue� �� UpdateSourceTrigger$ 	LostFocus=� �� ValidatesOnDataErrors .  �� ValidatesOnExceptions . {�5j      �� $. (CurrentCustomerOrder.Quantity.IsEditable�5k      �� 5o      $��2��6   $��5q�6*   $ 
Horizontal=�5p      .+�5q      �� $��DeliveryDate��6   $��Normal"�63   $��11(�6G   $	��150��6U   �� 5r      $��Normal"�6   $��11(�62   $	��200��6@    . #��[ 5w      {�.  ��5t      �� $1 +CurrentCustomerOrder.DeliveryDate.DataValue�$ 	LostFocus=� .  . 5x      5y      5{      5�      �� 5�   
   $ 
Horizontal=�6   $��2��6.   $��40,0,0,0q�6;   .+�5�      ��   GlassButton#��[ 6   $
��Save��5�       �� 	IsDefault . $��90��5�      $��30��6   $��Center=�5�      $��Center=�5�      $��5q�5�      ��5�      �� *SaveOrderCommand+5�      �� 5�      #��[ 6   $
��Edit��5�      $��90��5�      $��30��6   $��Center=�5�      $��Center=�5�      $��5q�5�      ��5�      �� *EditOrderCommand+5�      �� 5�      #��[ 6   $��Cancel��5�       �� IsCancel . $��90��5�      $��30��6   $��Center=�5�      $��Center=�5�      $��5q�5�      ��5�      �� *CancelOrderCommand+5�      5�      5�      5�      