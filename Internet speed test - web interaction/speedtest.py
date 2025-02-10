from selenium import webdriver
from selenium.webdriver.firefox.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By
from selenium.common.exceptions import TimeoutException
from selenium.webdriver.common.keys import Keys

import time
from datetime import datetime

#Load up webpage
options = Options() 
options.add_argument("-headless")





def writeToFile(download, upload, ip):

    filename = ip + ".txt"
    f = open(filename, "a")
    f.write(str(ip) + " (" + str(datetime.now().replace(microsecond = 0)) + '): ' + str(download) + 'mbps download, ' + str(upload) + 'mbps upload' + '\n')
    f.close()

def get_ip(driver):
    driver.get('https://www.whatismyip.com/')
    time.sleep(3)
    found = False
    while found == False:
        ip_address = driver.find_element(By.XPATH,"//span[@class='the-ipv4']").text
        if(len(ip_address) > 1):
            found = True
        time.sleep(1)
        
    return ip_address
            
        
        




def speedtest():
    driver = webdriver.Firefox(options=options)
    ip_address = get_ip(driver)

    driver.get('https://www.speedtest.net/')
    try:
        myElem = WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.ID, 'onetrust-accept-btn-handler')))
        #print("Page is ready!")


        #Click accept cookies
        driver.find_element(By.ID, 'onetrust-accept-btn-handler').click()

        #Click GO
        myElem = WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, 'start-button')))
        driver.find_element(By.CLASS_NAME, 'start-button').click()

        #Wait for results
        time.sleep(10)

        found = False
        while found == False:
            upload = driver.find_element(By.XPATH, "//span[@class='result-data-large number result-data-value upload-speed']").text
            if(len(upload) > 1):
                found = True
            time.sleep(3)


        download = driver.find_element(By.XPATH, "//span[@class='result-data-large number result-data-value download-speed']").text
        upload = driver.find_element(By.XPATH, "//span[@class='result-data-large number result-data-value upload-speed']").text

        #print(download)
        #print(upload)

        writeToFile(download, upload, ip_address)

        
    except TimeoutException:
        print("Something went wrong!")



valid_entry = False
while valid_entry == False:
    interval = int(input("Enter time interval between tests in minutes: "))
    if(interval > 1440 | interval < 1):
        print("Invalid time input")
    else:
        interval = interval * 60
        valid_entry = True

i = 1
while True:
    speedtest()
    print("Test" , i , "complete")
    i = i + 1
    time.sleep(interval)

    





