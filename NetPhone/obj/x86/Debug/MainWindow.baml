   M S B A M L   `   `   `  ����  NetPhone    NetPhone.MainWindow   5   
   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          �Name$  window��5   	    � Title$ 
MainWindow��5   	   $	��690��6   $	��340��6)      NoResize_window#���   5   	    � Icon$+ %/NetPhone;component/Images/111122.ico��6{   ��5	      �� 5
   
    �� ImageSource$) #/NetPhone;component/Images/件8.jpg��6    j� Stretch$ UniformToFill=�6E    j� TileMode$
 None=�6]   .��5      � $	��340��6   $	��690��6   .+�5   
   �� -   5         ��Name$ Minimize��5      $��10��64   $��284,6,46,674q�6@   ��5      �� 5      .R�5      ��  ��Name$	 img��6   $��/Images/Minimize.png��6)   $��10��6G   $��10��6R    �� Triggers 5      P� 5      	 P� Property	 �@J�IsMouseOver
 P� Value
 . . �5      ��  �� 
TargetName$	 img��6%    �� Property � �� �� Value$# /Images/MinimizeMouseOver.png��6H   5      5      ��5      �� 5      5      �� 5   
   -   5        $
 Exit��5      $��10��60   $��10��6;   $��
0,5,28,675q�6G   $��Right=�6[   ��5      �� 5      $* $/NetPhone;component/Images/close.png��6   $
 Fill=�6P   $
 None=�6_   ��5       �� 5!      .R�5"      �� $	 img��6   $��/Images/close.png��6)   $��10��6D   $��10��6O    5#      P� 5$      	 �@J�IsMouseOver
 . . �5%      �� $	 img��6%    � ��$  /Images/closeMouseOver.png��5&   5   5'      5)      5+      �� 5,   
   -   5        $��textBoxDisplayName��5,   W   $��34.999��6   $
��Left=�6"   $��75,279.55555,0,0q�6=   $	��Top=�6q   $	��200��6�   $��18(�6�   G� 5-   
   -   5        $��passwordBox��5-   Y   $��34.999��6   $
��Left=�6&   $��74,354.999,0,0q�6A   $	��Top=�6l   $	��201��6�   $��18(�6�   �� 5.   
   -   5        $��checkBoxPassword��5.   ]   $��记住密码��6   $��30��6"   $
��Left=�6.   $��
40,432,0,0q�6I   $	��Top=�6u   $��18(�6�   ���	#FFF5ECEC$	��100��6�   �� 5/   
   -   5        $��checkBoxAutoRun��5/   ]   $��自动登录��6   $��30��6"   $
��Left=�6.   $��
40,455,0,0q�6I   $	��Top=�6t   $��18(�6�   ���	#FFF2EFEF$	��100��6�   �� 50   
   -   5        $��labelReg��50   L   $��30��6   $
��Left=�6   $��197,322,0,0q�67   $	��Top=�6\   $	��100��6t   $��注册账号��6�   $��18(�6�   ���	#FFF5ECEC J� SnapsToDevicePixels .  $��Normal$�6�   $��Normal"�6�   $��Normal&�6�   6N  �� 51   
   -	   5        $��buttonLogIn��51   Y   $��登录��6   $��38��6   $
��Left=�6*   $��
99,528,0,0q�6E   $	��Top=�6l   $	��124��6�   ���	#FF0D0D0D$��15(�6�   ���	#FFFCFCFC6�   �� 52   
   -
   5        $��label1��52   �   $��忘记密码��6   $��18(�6   $��Normal&�6-   $��Normal$�6B   $��Normal"�6U   ���	#FFF5ECEC$��30��6�   $
��Left=�6�   $��193,391,0,0q�6�    .  $	��Top=�6�   $	��100��6�   $y�Hidden=�6y  53      54      