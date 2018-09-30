#!/usr/bin/python3
# -*- coding:utf8 -*-
import os


def run_model():
    """
        Run model
    :return:
    """
    run_cplex= "cplex -c \"read model_1.lp\"  \"Optimize\" "
    os.system(command=run_cplex)


def get_obj():
    """
    read and output obj
    :return:
    """

    with open('cplex.log', encoding='utf-8') as data_file:
        lines = data_file.readlines()

    # find optimal value in the Cplex log.
    optimal_solution = []
    with open('read_log.txt', 'wt') as f:
        for l in lines:
            words = l.split(":")
            # print("{0},".format(words[0]), file=f)
            if words[0] != "\n": # ignore blank line
                for w in range(0, len(words)):
                    print("{0}".format(words[w]), file=f, end='')
                if words[0] == "MIP - Integer optimal solution":
                    sol_str = words[1]
                    value_str = words[1].split("=")
                    val = value_str[1].split("\n")
                    optimal_solution.append(float(val[0]))

    with open('obj_val.txt', 'wt') as f:
        for i in range(0, len(optimal_solution)):
            print(optimal_solution[0], file=f)


if __name__ == "__main__":

    if os.path.exists("cplex.log"): # cplex.log seems to write append in cplex
        os.remove("cplex.log")
    # Step 1: set cplex model command
    run_model()
    # Step 2: read Cplex log
    get_obj()
