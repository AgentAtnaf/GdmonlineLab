import math
a = (input(" input a word ")) 

def Upper(a):
    upper = 0
    for c in a:
        if(c.isupper()):
             upper += upper  
    return upper
def Lower(a):
    lower = 0
    for c in a:
        if(c.islower()):
             lower +=  lower

Ans1 = Upper(a)
Ans2 = Lower(a)

print("Upper ",Ans1)
print("Lower ",Ans2)