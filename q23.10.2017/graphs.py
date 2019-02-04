from numpy import genfromtxt
import matplotlib.pyplot as plt
per_data=genfromtxt('automated.csv',delimiter=',')
plt.xlabel ('x stuff')
plt.ylabel ('y stuff')
plt.title('my test result')
plt.plot(per_data)
plt.show()
