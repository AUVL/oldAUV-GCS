using System;
using System.Linq;
using System.Collections.Generic;
using MissionPlanner.Utilities;



namespace PathProgram
{
    public class PathProgramming
    {
        static public List<PointLatLngAlt> dll_goallist = new List<PointLatLngAlt>();// target piont list(lat,lng,alt)
        static public List<PointLatLngAlt> dll_noflylist = new List<PointLatLngAlt>();// no-fly zone piont list(lat,lng,alt)
        static public List<PointLatLngAlt> dll_exnoflylist = new List<PointLatLngAlt>();// the turning point after enlarging the no-fly zone(lat,lng,alt)

        static int final_sol_no_ch = 3;//include tabu in_route(2nd) & cross_route (effect cpu time)
        static int tabulist_length = 10;// the Tabu list that it collected how many about the past optimal solution
        static int intabu_no_ch = 10;// in_route 
        static int crstabu_no_ch = 30;// cross_route
        static double exnofly_gain = 1.2;// no-fly zone enlarging rate ( exnofly_gain > 1)
        static int m_num = 1;// the amount of swarm (if undefine m_num = 1)
        static double exnofly_alt ; //turning point alt (1.start_alt 2.end_alt 3.avg_alt)
        static int latLength = dll_goallist.Count;// the amount of target piont 
        static int nflatLength = dll_noflylist.Count;// the amount of no-fly zone piont
        static double[,] Cij = new double[latLength, latLength];//the distance array of all lines
        static int[,] multi_path = new int[m_num, latLength - m_num + 3];// multi path
        static int[] point_num_of_multipath = new int[m_num];// the target piont amount of each swarm
        static int[] astar_ans = new int[nflatLength];
        static int[,] save_turning_point = new int[latLength * 2 + nflatLength, nflatLength + 2];
        static int[,] opt_multi_path = new int[m_num, latLength - m_num + 3];// array of final global solution
        static int[,] path_solution = new int[m_num, ((latLength - m_num + 3) - 1) * latLength + nflatLength];
        static int iterat = 0;// initial iteration value (can't be write)
        static int cw_or_ccw = 1;//cw=0 //ccw=1

        public void math(int groupset, List<PointLatLngAlt> Allpointlist, List<PointLatLngAlt> noflypointlist,
                         ref List<PointLatLngAlt> Apointlist, ref List<PointLatLngAlt> Bpointlist, 
                         ref List<PointLatLngAlt> Cpointlist,ref List<PointLatLngAlt> Dpointlist, ref List<PointLatLngAlt> Epointlist)
        {// main 
            
            List<PointLatLngAlt> final_list = new List<PointLatLngAlt>();// atter algorithm , number of target piont 
            List<PointLatLngAlt> turn_list = new List<PointLatLngAlt>();// atter algorithm , latitude and longitude of target piont 
            Apointlist.Clear();   Bpointlist.Clear();   Cpointlist.Clear();   Dpointlist.Clear();   Epointlist.Clear(); 
            dll_goallist.Clear(); final_list.Clear();   dll_noflylist.Clear();  dll_exnoflylist.Clear(); turn_list.Clear();// list clear

            for (int i = 0; i < Allpointlist.Count; i++)
            {//input target piont(lat,lng,alt)
                dll_goallist.Add(new PointLatLngAlt(Allpointlist[i].Lat, Allpointlist[i].Lng, Allpointlist[i].Alt));
            }

            for (int i = 0; i < noflypointlist.Count; i++)
            {//input no-fly zone piont(lat,lng,alt)
                dll_noflylist.Add(new PointLatLngAlt(noflypointlist[i].Lat, noflypointlist[i].Lng, noflypointlist[i].Alt));
            }
            if (dll_noflylist.Count != 0)
              dll_noflylist.Add(new PointLatLngAlt(noflypointlist[0].Lat, noflypointlist[0].Lng, noflypointlist[0].Alt));//add no-fly zone piont frist point for operation*

            ///// ↓variable definition↓ /////
            m_num = groupset;//input the amount of swarm
            latLength = dll_goallist.Count;
            if (dll_noflylist.Count == 0) nflatLength = dll_noflylist.Count;//if haven't no-fly zone
            else
            {//have no-fly zone(avoid amount = -1)
                nflatLength = dll_noflylist.Count - 1;
                exnofly();// calculate turning point*
            }
            Cij = new double[latLength, latLength];
            multi_path = new int[m_num, latLength - m_num + 3];
            point_num_of_multipath = new int[m_num];
            save_turning_point = new int[latLength * latLength + nflatLength , nflatLength + 2];
            opt_multi_path = new int[m_num, latLength - m_num + 3];
            path_solution = new int[m_num, ((latLength - m_num + 3) - 1) * 2 + nflatLength];
            astar_ans = new int[nflatLength];
            ///// ↑variable definition↑ /////

            distance();//cost function* ,calculate distance array of all lines* (if cross no-fly zone using A*)

            Array.Copy(improve(), opt_multi_path, opt_multi_path.Length);//the solution after tabu
            for (int i = 0; i < m_num; i++)
            {
                for (int j = 0; j < latLength - m_num + 3; j++)
                {//save number of target piont
                    path_solution[i, j] = opt_multi_path[i, j];
                }
            }

            if (dll_noflylist.Count != 0)
            {// if have no-fly zone
                insert();//insert turning point*

                for (int i = 0; i < dll_exnoflylist.Count; i++)
                {//add turning point into target piont list
                   dll_goallist.Add(new PointLatLngAlt(dll_exnoflylist[i].Lat, dll_exnoflylist[i].Lng, exnofly_alt));                              
                }
            }

            for (int i = 0; i < m_num; i++)
            {// output 
                for (int j = 0; j < dll_goallist.Count + 2; j++)//+2for L.106
                {//final solution , number -> lat,lng,alt
                    turn_list.Add(new PointLatLngAlt(dll_goallist[path_solution[i, j]].Lat, dll_goallist[path_solution[i, j]].Lng, dll_goallist[path_solution[i, j]].Alt));
                }
                
                for (int j = 0; j < turn_list.Count; j++)
                {// all swarm list
                    final_list.Add(new PointLatLngAlt(turn_list[j].Lat, turn_list[j].Lng, turn_list[j].Alt));

                    if (((turn_list[j].Lat == Allpointlist[0].Lat) && (turn_list[j].Lat == turn_list[j + 1].Lat) ) 
                      && ((turn_list[j].Lng == Allpointlist[0].Lng) && (turn_list[j].Lng == turn_list[j + 1].Lng))) break;// check on target amount with every swarm
                }

                if (i == 0)
                {// output swarm.A
                    if (cw_ccw(final_list[0].Lat, final_list[0].Lng, final_list[1].Lat, final_list[1].Lng, final_list[final_list.Count-2].Lat,final_list[final_list.Count - 2].Lng) )
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Apointlist.Add(new PointLatLngAlt(final_list[final_list.Count-1-j].Lat, final_list[final_list.Count-1-j].Lng, final_list[final_list.Count-1-j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Apointlist.Add(new PointLatLngAlt(final_list[j].Lat, final_list[j].Lng, final_list[j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                }
                if (i == 1)
                {// output swarm.B
                    if (cw_ccw(final_list[0].Lat, final_list[0].Lng, final_list[1].Lat, final_list[1].Lng, final_list[final_list.Count - 2].Lat, final_list[final_list.Count - 2].Lng))
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Bpointlist.Add(new PointLatLngAlt(final_list[final_list.Count - 1 - j].Lat, final_list[final_list.Count - 1 - j].Lng, final_list[final_list.Count - 1 - j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Bpointlist.Add(new PointLatLngAlt(final_list[j].Lat, final_list[j].Lng, final_list[j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                }

                if (i == 2)
                {// output swarm.C
                    if (cw_ccw(final_list[0].Lat, final_list[0].Lng, final_list[1].Lat, final_list[1].Lng, final_list[final_list.Count - 2].Lat, final_list[final_list.Count - 2].Lng))
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Cpointlist.Add(new PointLatLngAlt(final_list[final_list.Count - 1 - j].Lat, final_list[final_list.Count - 1 - j].Lng, final_list[final_list.Count - 1 - j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Cpointlist.Add(new PointLatLngAlt(final_list[j].Lat, final_list[j].Lng, final_list[j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                }
                if (i == 3)
                {// output swarm.D
                    if (cw_ccw(final_list[0].Lat, final_list[0].Lng, final_list[1].Lat, final_list[1].Lng, final_list[final_list.Count - 2].Lat, final_list[final_list.Count - 2].Lng))
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Dpointlist.Add(new PointLatLngAlt(final_list[final_list.Count - 1 - j].Lat, final_list[final_list.Count - 1 - j].Lng, final_list[final_list.Count - 1 - j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Dpointlist.Add(new PointLatLngAlt(final_list[j].Lat, final_list[j].Lng, final_list[j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                }
                if (i == 4)
                {// output swarm.E
                    if (cw_ccw(final_list[0].Lat, final_list[0].Lng, final_list[1].Lat, final_list[1].Lng, final_list[final_list.Count - 2].Lat, final_list[final_list.Count - 2].Lng))
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Epointlist.Add(new PointLatLngAlt(final_list[final_list.Count - 1 - j].Lat, final_list[final_list.Count - 1 - j].Lng, final_list[final_list.Count - 1 - j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < final_list.Count; j++)
                        {
                            Epointlist.Add(new PointLatLngAlt(final_list[j].Lat, final_list[j].Lng, final_list[j].Alt));
                        }
                        turn_list.Clear();
                        final_list.Clear();
                    }
                }
            }

        }
        
        static private int[,] improve()//tabu improve* /i: path after cut /o: final path solution without turning point
        {
            multi_path = initail();//1st TABU in_route -> cut finish

            int[,] opt_path = new int[m_num, latLength - m_num + 3];// optimal path
            Array.Copy(multi_path, opt_path, multi_path.Length);
            double opt_max = 0;// optimal single path distance 
            double opt_total = 0;// sum all swarm distance
            for (int i = 0; i < m_num; i++)
            {
                int[] path_crs_tabu = new int[latLength - m_num + 3];
                for (int j = 0; j < latLength - m_num + 3; j++)
                {
                    path_crs_tabu[j] = opt_path[i, j];
                }
                double dist_f = path_distance(path_crs_tabu);// calculate single path when cross_route working
                opt_total += dist_f;// sum optimal total distance 
                if (dist_f > opt_max) opt_max = dist_f;// if (new single path distance > optimal single path distance) replace
            } 
            int count = 0;

            do
            {
                Array.Copy(Tabu_cross_route(multi_path), multi_path, multi_path.Length);// tabu cross_route

                for (int i = 0; i < m_num; i++)
                {//aimed at all single path , and improve
                    int[] single_path_of_multi = new int[point_num_of_multipath[i] + 2];
                    for (int j = 0; j < point_num_of_multipath[i] + 2; j++)// take path distance
                    {
                        single_path_of_multi[j] = multi_path[i, j];
                    }

                    Array.Copy(Tabu_in_route(single_path_of_multi), single_path_of_multi, single_path_of_multi.Length);//2nd tabu in_route 

                    for (int j = 0; j < point_num_of_multipath[i] + 2; j++)
                    {
                        multi_path[i, j] = single_path_of_multi[j];
                    }
                }//

                double cur_max = 0;// current single distance
                double cur_total = 0;// current total distance
                for (int i = 0; i < m_num; i++)
                {
                    int[] path_f = new int[latLength - m_num + 3];
                    for (int j = 0; j < latLength - m_num + 3; j++)
                    {
                        path_f[j] = multi_path[i, j];
                    }
                    double dist_f = path_distance(path_f);
                    cur_total += dist_f;
                    if (dist_f > cur_max) cur_max = dist_f;
                }
                
                if (compare_max_total(cur_max, cur_total, opt_max, opt_total))
                {// admit the best of solution
                    opt_max = cur_max;
                    opt_total = cur_total;
                    Array.Copy(multi_path, opt_path, multi_path.Length);
                    count = 0;
                }
                else
                {
                    count++;
                }
                iterat++;// iterat ++
            } while (!(count == final_sol_no_ch));
            
            return opt_path;
        }

        static private int[,] initail()//cut route and change to path after 1st tabu in_route* 
        {
            int[] single_path = new int[latLength + 1];// single path
            Array.Copy(Tabu_in_route(NDM()), single_path, latLength + 1);// use near distance method than tabu in_route search*
            return cut(single_path);// route cut*
        }

        static private int[,] cut(int[] single_path)//route cut* 
        {// i: all target point / o: initial path 
            int[] cut_path = new int[0];
            double cut_path_d = 0;
            double cut_index_d = path_distance(single_path) / m_num;// (total distance) divided (swarm amount) , average distance*
            for (int i = 0; i < m_num - 1; i++)
            {
                int j = 1;
                do{
                    j++;
                    cut_path = new int[j];
                    Array.Copy(single_path, cut_path, j);
                    cut_path_d = path_distance(cut_path);//after cut , the path distance (does not include return home)
                } while (!((cut_path_d > cut_index_d))); //the condition of shutting down , (path distance)>(average distance)

                if (cut_path.Length == 2)
                {// if get frist point the path distance more than average distance , "take one target point" at least*
                    for (int k = 0; k < cut_path.Length; k++)
                    {
                        multi_path[i, k] = cut_path[k];
                    }
                    for (int k = 1; k < single_path.Length - cut_path.Length + 1; k++)
                    {// move forward the remaining target point 
                        single_path[k] = single_path[cut_path.Length - 1 + k];
                    }
                    point_num_of_multipath[i] = j - 1;// group 1st ~ (swarm amount -1) target point 
                }
                else
                {//normal cut
                    for (int k = 0; k < cut_path.Length - 1; k++)
                    {
                        multi_path[i, k] = cut_path[k];
                    }
                    for (int k = 1; k < single_path.Length - cut_path.Length + 2; k++)
                    {
                        single_path[k] = single_path[cut_path.Length - 2 + k];
                    }
                    point_num_of_multipath[i] = j - 2;
                }

                multi_path[i, cut_path.Length] = 0;//add home point at path least

                Array.Resize(ref single_path, single_path.Length - cut_path.Length + 2);// adjust array size
            }

            for (int i = 0; i < single_path.Length; i++)
            {
                multi_path[m_num - 1, i] = single_path[i];// single path 
            }
            int ii = 1;
            do
            {
                ii++;
            } while (!(multi_path[m_num - 1, ii] == 0));
            point_num_of_multipath[m_num - 1] = ii - 1;// last group take all the remaining target point

            return multi_path;
        }

        static private int insert()// insert turning point* 
        {// i: the route after tabu / o: final path solution
            for (int i = 0; i < m_num; i++)
            {
                int[] path = new int[((latLength - m_num + 3) - 1) * nflatLength];
                for (int j = 0; j < path_solution.GetLength(1); j++)
                {
                    path[j] = path_solution[i, j];
                }
                int k = 0;
                int n = 0;
                do {
                    n = 0;
                    for (int l = 0; l < save_turning_point.GetLength(0); l++)
                    {//scanning row
                        if (path[k + n] == save_turning_point[l, 0] && path[k + n + 1] == save_turning_point[l, 1])
                        {//start and end point from distance program ,scan the route cross no-fly zone or not
                            int f = 2;
                            do {
                                f++;
                            } while (!(save_turning_point[l, f] == 0));
                            int[] path1 = new int[((latLength - m_num + 3) - 1) * nflatLength];
                            Array.Copy(path, path1, path1.Length);
                            for (int m = 0; m < path_solution.GetLength(1) - (k + 1) - (f - 2); m++)
                            {// which point to meet no-fly zone , move all begind its point backward 
                                path[k + n + 1 + (f - 2) + m] = path1[k + n + 1 + m];
                            }
                            for (int m = 0; m < f - 2; m++)
                            {// insert turning point and put last path back
                                path[k + n + 1 + m] = save_turning_point[l, 2 + m];
                            }
                            n += f - 2;

                            exnofly_alt = dll_goallist[save_turning_point[l, 1]].Alt;// exnofly_alt = start.alt 
                            if (exnofly_alt == 0)
                            {
                                exnofly_alt = dll_goallist[save_turning_point[l, 0]].Alt;// if exnofly_alt = 0 , exnofly_alt = end.alt 
                            }
                            break;
                        }
                        
                    }
                    k++;
                    for (int j = 0; j < path_solution.GetLength(1); j++)
                    {// final path solution
                        path_solution[i, j] = path[j];
                    }
                } while (!(path[k + n] == 0));
            }
            return 0;
        }

        static private int[,] Tabu_cross_route(int[,] multi_path)//TABU cross_route*
        {//i: the path after cut /o: all path 
            int[,] current_solution = new int[multi_path.GetLength(0), multi_path.GetLength(1)];// initail current solution
            Array.Copy(multi_path, current_solution, multi_path.Length);

            double[,] tabulist = new double[tabulist_length, 2];//mix & total put inset tabu_list 
            int tabu_cnt = 0;//tabulist iterat
            int[,] global_opt_path = new int[multi_path.GetLength(0), multi_path.GetLength(1)];
            Array.Copy(current_solution, global_opt_path, current_solution.Length);
            double global_opt_max = 0;
            double global_opt_total = 0;
            for (int i = 0; i < m_num; i++)
            {
                int[] path_global = new int[latLength - m_num + 3];
                for (int j = 0; j < latLength - m_num + 3; j++)
                {
                    path_global[j] = global_opt_path[i, j];
                }
                double dist_f = path_distance(path_global);// calculate path distance 
                global_opt_total += dist_f;// sum distance
                if (dist_f > global_opt_max) global_opt_max = dist_f;// focus on improve bad solsution
            }

            int[,] global_sub_path = new int[multi_path.GetLength(0), multi_path.GetLength(1)];
            Array.Copy(current_solution, global_sub_path, current_solution.Length); // avoid local optimization*
            double global_sub_max = double.MaxValue;// sencond best 
            double global_sub_total = double.MaxValue;

            double initail_max = 0;
            double initail_total = 0;
            for (int i = 0; i < m_num; i++)
            {
                int[] path_initail = new int[latLength - m_num + 3];
                for (int j = 0; j < latLength - m_num + 3; j++)
                {
                    path_initail[j] = multi_path[i, j];
                }
                double dist_initail = path_distance(path_initail);
                initail_total += dist_initail;
                if (dist_initail > initail_max) initail_max = dist_initail;
            }
            
            int count = 0;
            do
            {//move method (2-OPT)
                int[,] near_opt_path = new int[multi_path.GetLength(0), multi_path.GetLength(1)];
                double near_opt_max = double.MaxValue;
                double near_opt_total = double.MaxValue;

                for (int i = 0; i < m_num - 1; i++)// main group
                {
                   for (int j = 0; j < m_num - 1 - i; j++)// second group 
                   {
                      for (int k = 0; k < point_num_of_multipath[i] + 1; k++)//main group cut when k point
                      {
                         int[] first_cut_head_path = new int[k + 1];//main group front
                         int[] first_cut_tail_path = new int[point_num_of_multipath[i] + 1 - k];//main group rear
                         for (int l = 0; l < k + 1; l++)
                         {
                          first_cut_head_path[l] = current_solution[i, l];
                         }
                         for (int l = 0; l < point_num_of_multipath[i] + 1 - k; l++)
                         {
                          first_cut_tail_path[l] = current_solution[i, k + l + 1];
                         }

                         for (int l = 0; l < point_num_of_multipath[i + j + 1] + 1; l++)//second group cut when l point
                         {
                            int[] second_cut_head_path = new int[l + 1];//second group front
                            int[] second_cut_tail_path = new int[point_num_of_multipath[i + j + 1] + 1 - l];//second group rear
                            for (int m = 0; m < l + 1; m++)
                            {
                             second_cut_head_path[m] = current_solution[i + j + 1, m];
                            }
                            for (int m = 0; m < point_num_of_multipath[i + j + 1] + 1 - l; m++)
                            {
                             second_cut_tail_path[m] = current_solution[i + j + 1, l + m + 1];
                            }

                             int[,] multi_path_forward = new int[m_num, latLength - m_num + 3];//forward path
                             Array.Copy(current_solution, multi_path_forward, current_solution.Length);
                             int[,] multi_path_reverse = new int[m_num, latLength - m_num + 3];//reverse path
                             Array.Copy(current_solution, multi_path_reverse, current_solution.Length);

                            //forward rule
                            for (int m = 0; m < k + 1; m++)//main group front link up second group rear
                            {
                             multi_path_forward[i, m] = first_cut_head_path[m];
                            }
                            for (int m = 0; m < point_num_of_multipath[i + j + 1] + 1 - l; m++)
                            {
                             multi_path_forward[i, m + k + 1] = second_cut_tail_path[m];
                            }
                            for (int m = 0; m < multi_path_forward.GetLength(1) - (k + point_num_of_multipath[i + j + 1] + 2 - l); m++)
                            {
                              multi_path_forward[i, point_num_of_multipath[i + j + 1] - l + k + 2 + m] = 0;//clear last point 
                            }

                            for (int m = 0; m < l + 1; m++)//second group front link up main group rear
                            {
                              multi_path_forward[i + j + 1, m] = second_cut_head_path[m];
                            }
                            for (int m = 0; m < point_num_of_multipath[i] + 1 - k; m++)
                            {
                              multi_path_forward[i + j + 1, m + l + 1] = first_cut_tail_path[m];
                            }
                            for (int m = 0; m < multi_path_forward.GetLength(1) - (l + point_num_of_multipath[i] + 2 - k); m++)
                            {
                              multi_path_forward[i + j + 1, point_num_of_multipath[i] + 1 - k + l + 1 + m] = 0;//clear last point 
                            }

                            //reverse rule 
                            for (int m = 0; m < k + 1; m++)//main group front link up reverse the second group front
                            {
                              multi_path_reverse[i, m] = first_cut_head_path[m];
                            }
                            for (int m = 0; m < l + 1; m++)
                            {
                              multi_path_reverse[i, m + k + 1] = second_cut_head_path[l - m];
                            }
                            for (int m = 0; m < multi_path_forward.GetLength(1) - (k + l + 2); m++)
                            {
                              multi_path_reverse[i, k + 2 + l + m] = 0;//clear last point 
                            }

                            for (int m = 0; m < point_num_of_multipath[i + j + 1] + 1 - l; m++)//reverse the second group rear link up main group rear
                            {
                              multi_path_reverse[i + j + 1, m] = second_cut_tail_path[point_num_of_multipath[i + j + 1] - l - m];
                            }
                            for (int m = 0; m < point_num_of_multipath[i] + 1 - k; m++)
                            {
                              multi_path_reverse[i + j + 1, m + point_num_of_multipath[i + j + 1] + 1 - l] = first_cut_tail_path[m];
                            }
                            for (int m = 0; m < multi_path_forward.GetLength(1) - (point_num_of_multipath[i + j + 1] + 1 - l + (point_num_of_multipath[i] + 1 - k)); m++)
                            {
                              multi_path_reverse[i + j + 1, (point_num_of_multipath[i + j + 1] + 1 - l + (point_num_of_multipath[i] + 1 - k)) + m] = 0;//clear last point 
                            }

                                double max_dist_f = 0;// max distance of forward path 
                                double total_dist_f = 0;// total distance of forward path 
                                double max_dist_r = 0;// max distance of reverse path 
                                double total_dist_r = 0;// total distance of reverse path 
                                bool not_feasible_f = false;
                                bool not_feasible_r = false;
                                for (int m = 0; m < m_num; m++)
                                {
                                    int[] path_f = new int[latLength - m_num + 3];//forward path
                                    int[] path_r = new int[latLength - m_num + 3];//reverse path
                                    for (int n = 0; n < latLength - m_num + 3; n++)
                                    {
                                        path_f[n] = multi_path_forward[m, n];
                                        path_r[n] = multi_path_reverse[m, n];
                                    }
                                    double dist_f = path_distance(path_f);//calculate path distance 
                                    double dist_r = path_distance(path_r);
                                    if (dist_f == 0) not_feasible_f = true;//if distance = 0 , disuse the solution
                                    if (dist_r == 0) not_feasible_r = true;
                                    total_dist_f += dist_f;
                                    total_dist_r += dist_r;
                                    if (dist_f > max_dist_f) max_dist_f = dist_f;   
                                    if (dist_r > max_dist_r) max_dist_r = dist_r; //focus on which group solsution is worst,improve it (load balance)*
                                }

                                for (int m = 0; m < tabulist_length; m++)
                                {//judge this soiution can be use or not (distance != 0 & exclude the tabu list)
                                    not_feasible_f |= tabulist[m, 0] == max_dist_f && tabulist[m, 1] == total_dist_f;
                                    not_feasible_r |= tabulist[m, 0] == max_dist_r && tabulist[m, 1] == total_dist_r;
                                }
                                
                                if (compare_max_total(max_dist_f, total_dist_f, max_dist_r, total_dist_r))//max total (select forward or reverse solution)
                                {// forward win
                                    if (not_feasible_f == true)
                                    {// forward disuse
                                        if (not_feasible_r == true)
                                        {// reverse disuse 
                                            //find next iterat
                                        }
                                        else
                                        {// only reverse can be use
                                            if (compare_max_total(max_dist_r, total_dist_r, near_opt_max, near_opt_total))
                                            {
                                                near_opt_max = max_dist_r;
                                                near_opt_total = total_dist_r;
                                                Array.Copy(multi_path_reverse, near_opt_path, multi_path_reverse.Length);
                                            }
                                        }
                                    }
                                    else
                                    {// forward win and can be use
                                        if (compare_max_total(max_dist_f, total_dist_f, near_opt_max, near_opt_total))
                                        {
                                            near_opt_max = max_dist_f;
                                            near_opt_total = total_dist_f;
                                            Array.Copy(multi_path_forward, near_opt_path, multi_path_forward.Length);
                                        }
                                    }
                                }
                                else
                                {// reverse win
                                    if (not_feasible_r == true)
                                    {// reverse disuse
                                        if (not_feasible_f == true)
                                        {//forward disuse
                                            //find next iterat
                                        }
                                        else
                                        {//only forward can be use
                                            if (compare_max_total(max_dist_f, total_dist_f, near_opt_max, near_opt_total))
                                            {
                                                near_opt_max = max_dist_f;
                                                near_opt_total = total_dist_f;
                                                Array.Copy(multi_path_forward, near_opt_path, multi_path_forward.Length);
                                            }
                                        }
                                    }
                                    else
                                    {// reverse win and can be use
                                        if (compare_max_total(max_dist_r, total_dist_r, near_opt_max, near_opt_total))
                                        {
                                            near_opt_max = max_dist_r;
                                            near_opt_total = total_dist_r;
                                            Array.Copy(multi_path_reverse, near_opt_path, multi_path_reverse.Length);
                                        }
                                    }
                                }
                         }
                        }
                    }
                }
                Array.Copy(near_opt_path, current_solution, near_opt_path.Length);
                for (int i = 0; i < m_num; i++)
                {
                    int j = 0;
                    do
                    {
                        j++;
                    } while (!(current_solution[i, j] == 0));
                    point_num_of_multipath[i] = j - 1;
                }

                tabulist[tabu_cnt, 0] = near_opt_max;//tabu list
                tabulist[tabu_cnt, 1] = near_opt_total;
                tabu_cnt++;
                if (tabu_cnt == tabulist_length) tabu_cnt = 0;

                if (compare_max_total(near_opt_max, near_opt_total, global_opt_max, global_opt_total))
                {//if new solution better than global solution , replace
                    global_opt_max = near_opt_max;
                    global_opt_total = near_opt_total;
                    Array.Copy(near_opt_path, global_opt_path, near_opt_path.Length);
                    count = 0;
                }
                else
                {
                    count++;
                    if (compare_max_total(near_opt_max, near_opt_total, global_sub_max, global_sub_total) && (near_opt_max != global_opt_max || near_opt_total != global_opt_total))
                    {//avoid local optimization , use sencond best solution
                        global_sub_max = near_opt_max;
                        global_sub_total = near_opt_total;
                        Array.Copy(near_opt_path, global_sub_path, near_opt_path.Length);
                    }
                }
                
            } while (!(count == crstabu_no_ch));
            if (iterat >= 1)
            {
                if ((global_opt_max == initail_max) && (global_opt_total == initail_total))
                {
                    Array.Copy(global_sub_path, global_opt_path, global_sub_path.Length);
                }
            }

            for (int i = 0; i < m_num; i++)
            {
                int j = 0;
                do{
                    j++;
                } while (!(global_opt_path[i, j] == 0));
                point_num_of_multipath[i] = j - 1;
            }
            return global_opt_path;
        }

        static private int[] Tabu_in_route(int[] ndm) //tabu in_route*
        {
            int[] current_solution = new int[ndm.Length]; //current solution
            Array.Copy(ndm, current_solution, ndm.Length);

            double[] tabulist = new double[tabulist_length];
            int L = 0;//iterat
            int[] sol_opt_path = new int[ndm.Length];//global solution of single path
            Array.Copy(ndm, sol_opt_path, ndm.Length);
            double opt_path_d = path_distance(sol_opt_path);
            int no_change = 0;

            do
            {
                int[] near_opt_path = new int[ndm.Length]; //nearly optimal path
                double near_opt_d = double.MaxValue; //nearly optimal distance
                for (int i = 0; i < ndm.Length - 3; i++) //planning path follow the rule
                {
                    int[] path_1 = new int[ndm.Length];
                    for (int j = 0; j < i + 1; j++)
                    {//rule_path_step1(keep the number,and carry +1 +1 ...)
                        path_1[j] = current_solution[j];
                    }

                    int Nj;
                    if (i < 2) Nj = ndm.Length - 4;
                    else Nj = ndm.Length - 1 - (i + 2);

                    for (int j = 0; j < Nj; j++)
                    {
                        for (int k = 0; k < j + 2; k++)
                        {//rule_path_step2(reverse the number ,carry +2 +2 ...)
                            path_1[i + 1 + k] = current_solution[i + j + 2 - k];
                        }

                        for (int k = 0; k < ndm.Length - 1 - (i + j + 2); k++)
                        {//rule_path_step3(the last point is constant)
                            path_1[i + j + 3 + k] = current_solution[i + j + k + 3];
                        }

                        double path_1_d = path_distance(path_1);
                        bool in_list = false;
                        for (int k = 0; k < tabulist.Length; k++)
                        {
                            in_list |= (path_1_d == tabulist[k]);
                        }

                        if ((path_1_d < near_opt_d) && !(in_list)) 
                        {
                            near_opt_d = path_1_d;
                            Array.Copy(path_1, near_opt_path, path_1.Length);                            
                        }
                    }
                }

                tabulist[L] = near_opt_d;// put optimal solution in the tabu_list
                L++;
                if (L == tabulist_length) L = 0;// come in frist and out frist 

                if (near_opt_d < opt_path_d)
                {
                    opt_path_d = near_opt_d;
                    Array.Copy(near_opt_path, sol_opt_path, ndm.Length);
                    no_change = 0;//if solution updated, no_change iterat = 0
                }
                else no_change++;
                Array.Copy(near_opt_path, current_solution, near_opt_path.Length);

            } while (!(no_change == intabu_no_ch));//stop situation(no_change iterat = intabu_no_ch)
            return sol_opt_path;
        }

        static private double path_distance(int[] path) //calculate single path distance*
        {
            double value;
            int path_length = path.Length - 1;
            double path_dist = 0;
            for (int i = 0; i < path_length; i++)
            {//sum target point distance 
                value = Cij[path[i], path[i + 1]];
                if (value >= double.MaxValue) value = 0;
                path_dist += value;
            }
            if (path_dist == 0) path_dist = double.MaxValue;

            return path_dist;
        }

        static private int[] NDM() //Nearly Distance Method* 
        {// just take row point = all target point
            int[] NDM_path = new int[latLength + 1];
            NDM_path[0] = 0;
            double[,] Cij_copy = new double[Cij.GetLength(0), Cij.GetLength(1)];
            Array.Copy(Cij, Cij_copy, Cij.Length);//copy distance array
            
            for (int i = 0; i < latLength; i++)
            {
                Cij_copy[0, i] = double.MaxValue;// row No.zero = MaxValue
            }
            for (int i = 0; i < latLength - 1; i++)
            {
                double min_value = double.MaxValue; 
                int min_row = int.MaxValue;
                for (int j = 0; j < latLength; j++)
                {
                    for (int k = 0; k < latLength; k++)
                    {
                        if (Cij_copy[j, k] < min_value)// take array.min
                        {
                            min_value = Cij_copy[j, k];
                            min_row = j;
                        }
                    }
                }
                for (int j = 0; j < latLength; j++)
                {
                    Cij_copy[min_row, j] = double.MaxValue; // array.min = maxvalue
                }
                NDM_path[i + 1] = min_row; // initail route
            }

            NDM_path[latLength] = 0;
            return NDM_path;
        }

        static private bool compare_max_total(double a_max, double a_total, double b_max, double b_total)//compare (adjust load balance or total distance minimum)* (near max , near total,global max , global total) 
        {// i: now solution &global solution / o: true or false 
            bool ask_tf = false;
            if (a_max < b_max) ask_tf = true;//
            else
            {
                if (a_max == b_max)
                {
                    if (a_total < b_total) ask_tf = true;
                    else ask_tf = false;
                }
                else ask_tf = false;
            }
            return ask_tf;
        }

        static private double[,] distance() //calculate distance array of all lines* astar* (if cross no-fly zone using astar)
        {// i: target & no-fiy zone point / o: distance array of all lines
            int count = 0;
            for (int i = 0; i < Cij.GetLength(0); i++)
            {
                for (int j = 0; j < Cij.GetLength(0); j++)
                {
                    bool x = false;

                    for (int k = 0; k < nflatLength; k++)
                    {// judge the target iine cross no-fly zone or not 
                        x = x || crsnofly(dll_goallist[i].Lat, dll_goallist[i].Lng, dll_goallist[j].Lat, dll_goallist[j].Lng,
                                          dll_noflylist[k].Lat, dll_noflylist[k].Lng, dll_noflylist[k + 1].Lat, dll_noflylist[k + 1].Lng);
                    }
                    if (x == false)
                    {// normal distance
                        Cij[i, j] = dist(dll_goallist[i].Lat, dll_goallist[i].Lng, dll_goallist[j].Lat, dll_goallist[j].Lng);
                    }
                    else
                    {// cross no-fly zone 
                        Cij[i, j] = astar(dll_goallist[i].Lat, dll_goallist[i].Lng, dll_goallist[i].Alt, dll_goallist[j].Lat, dll_goallist[j].Lng, dll_goallist[j].Alt); //astar
                        save_turning_point[count, 0] = i;//because cross no-fly zone ,so save this starting point for insert function
                        save_turning_point[count, 1] = j;//end point
                       
                        int f = 1;
                        do
                        {
                            f++;
                        } while (!(astar_ans[f] == 0));

                        for (int k = 0; k < f - 1; k++)
                        {// need through which turning point
                            save_turning_point[count, 2 + k] = astar_ans[f - 1 - k] + latLength - 1;
                        }
                        count++;
                    }
                }
            }
            return Cij;
        }

        static private bool crsnofly(double lat0, double lng0, double lat1, double lng1, double lat2, double lng2, double lat3, double lng3)//judge cross or not
        {
            {
                double d1 = crsproduct(lat2, lng2, lat3, lng3, lat0, lng0);//3,4,1
                double d2 = crsproduct(lat2, lng2, lat3, lng3, lat1, lng1);//3,4,2
                double d3 = crsproduct(lat0, lng0, lat1, lng1, lat2, lng2);//1,2,3
                double d4 = crsproduct(lat0, lng0, lat1, lng1, lat3, lng3);//1,2,4

                if (d1 * d2 < 0 && d3 * d4 < 0) return true; //cross

                else if (d1 == 0 && sameline(lat2, lng2, lat3, lng3, lat0, lng0)) return true;//cross

                else if (d2 == 0 && sameline(lat2, lng2, lat3, lng3, lat1, lng1)) return true;//cross

                else if (d3 == 0 && sameline(lat0, lng0, lat1, lng1, lat2, lng2)) return true;//cross

                else if (d4 == 0 && sameline(lat0, lng0, lat1, lng1, lat3, lng3)) return true;//cross

                else return false; //no cross
            }
        }

        static private double crsproduct(double lat0, double lng0, double lat1, double lng1, double noflylat0, double noflylng0) //Cross product
        {
            return ((noflylat0 - lat0) * (lng1 - lng0)) - ((lat1 - lat0) * (noflylng0 - lng0));// Cross product array simplify
        }

        static private bool sameline(double lat0, double lng0, double lat1, double lng1, double noflylat0, double noflylng0) //same line (overlap)
        {
            double minx = Math.Min(lat0, lat1);
            double maxx = Math.Max(lat0, lat1);
            double miny = Math.Min(lng0, lng1);
            double maxy = Math.Max(lng0, lng1);

            return noflylat0 >= minx && noflylat0 <= maxx && noflylng0 >= miny && noflylng0 <= maxy; //return true or false
        }

        static private double dist(double lat0, double lng0, double lat1, double lng1)//distance formula
        {// i: any two point / o: distance
            double X = Math.Cos(lat0 * Math.PI / 180) * Math.Cos(lng0 * Math.PI / 180) -
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lng1 * Math.PI / 180);

            double Y = Math.Cos(lat0 * Math.PI / 180) * Math.Sin(lng0 * Math.PI / 180) -
                       Math.Cos(lat1 * Math.PI / 180) * Math.Sin(lng1 * Math.PI / 180);

            double Z = Math.Sin(lat0 * Math.PI / 180) - Math.Sin(lat1 * Math.PI / 180);

            double Tij = 2 * 6378137 * Math.Asin(Math.Sqrt(Math.Pow(X, 2) +
                                                           Math.Pow(Y, 2) +
                                                           Math.Pow(Z, 2)) / 2); // Great-circle distance formula (Equatorial radius = 6378137m)

            if (Tij == 0) return double.MaxValue;

            else return Tij;
        }

        static private double exnofly()//the turning point after enlarging the no-fly zone
        {
            double pclat = ((dll_noflylist.Sum(sumlat => sumlat.Lat)) - (dll_noflylist[dll_noflylist.Count - 1].Lat)) / (dll_noflylist.Count - 1);//lat center
            double pclng = ((dll_noflylist.Sum(sumlng => sumlng.Lng)) - (dll_noflylist[dll_noflylist.Count - 1].Lng)) / (dll_noflylist.Count - 1);//lng center
            for (int i = 0; i < dll_noflylist.Count - 1; i++)
            {
                dll_exnoflylist.Add(new PointLatLngAlt(pclat + exnofly_gain * (dll_noflylist[i].Lat - pclat), pclng + exnofly_gain * (dll_noflylist[i].Lng - pclng)));// pc + exnofly_gain*(pi-pc)
            }
            return 0;
        }

        static private double astar(double lat0, double lng0, double alt0, double lat1, double lng1, double alt1) // A*
        {// i: start & end piont  / o: distance of dodge no-fly zone
            
            List<PointLatLngAlt> dll_turnlist = new List<PointLatLngAlt>();

            dll_turnlist.Add(new PointLatLngAlt(lat0, lng0, alt0));//start point
            for (int i = 0; i < nflatLength; i++)
            {
                dll_turnlist.Add(new PointLatLngAlt(dll_exnoflylist[i].Lat, dll_exnoflylist[i].Lng));//turning point
            }
            dll_turnlist.Add(new PointLatLngAlt(lat1, lng1, alt1));//end point

            //

            double[,] cij = new double[nflatLength + 2, nflatLength + 2];
            double[] tlat = new double[dll_exnoflylist.Count + 2];//(start, turning0,,,, end)
            double[] tlng = new double[dll_exnoflylist.Count + 2];
            for (int i = 0; i < dll_turnlist.Count; i++)
            {
                tlat[i] = dll_turnlist[i].Lat;
                tlng[i] = dll_turnlist[i].Lng;
            }

            cij = smalldistance(tlat, tlng);//small distance array

            //
            double[] h = new double[cij.GetLength(0)];
            for (int i = 0; i < cij.GetLength(0); i++)//H.function
            {
                h[i] = dist(tlat[tlat.Length - 1], tlng[tlng.Length - 1], tlat[i], tlng[i]);//H.Funtion
                if (h[i] == double.MaxValue) h[i] = 0;//if goal, make H.function = 0
            }

            //
            double[,] open = new double[nflatLength * nflatLength + latLength, nflatLength * nflatLength + latLength]; 
            double[,] close = new double[nflatLength * nflatLength + latLength, nflatLength * nflatLength + latLength];
            double[,] de = new double[nflatLength * nflatLength + latLength, nflatLength * nflatLength + latLength];

            //////////////////////////////search target//////////////////////////////////////
            int C = int.MaxValue;
            double[] C_info = new double[4];
            bool reach_target = false;
            double abs = 0;
            double astar_g = 0;
            int asg = 0;

            do
            {
                double f = double.MaxValue;
                int mini_index = int.MaxValue;
                bool noloop = true;

                for (int i = 0; i < open.GetLength(0); i++) //save open.array [i,3] 
                {
                    if (open[i, 3] < f)  // get F.function min
                    {
                        mini_index = i;
                        f = open[i, 3];
                    }
                }

                for (int i = 0; i < 4; i++) // take f.min row from open.array
                {
                    C_info[i] = open[mini_index, i];
                }
                open = delete(open, mini_index);// delete open C row
                C = Convert.ToInt32(C_info[0]);

                double[,] m = new double[0, 4];

                for (int i = 0; i < cij.GetLength(0); i++) // {open,close,de,C_info}
                {
                    bool a = false;
                    for (int j = 0; j < de.GetLength(0); j++) // dead end 
                    {
                        a |= (de[j, 0] == i);
                    }
                    if (a)
                    { }

                    else
                    {
                        bool b = false;
                        for (int j = 0; j < close.GetLength(0); j++) //close
                        {
                            b |= (close[j, 0] == i);
                        }
                        if (b)
                        { }
                        else
                        {
                            bool c = false;
                            int loop = 0;
                            for (int j = 0; j < open.GetLength(0); j++) //open
                            {
                                c |= (open[j, 0] == i);
                                if (open[j, 0] == i) loop = j;//open current point overlap -> keep point.j in loop
                            }
                            if (c)
                            {
                                if (cij[C, i] + C_info[2] + h[i] < open[loop, 3]) //if overlap , retain better one (F < open.F)
                                {
                                    open[loop, 0] = i;
                                    open[loop, 1] = C;
                                    open[loop, 2] = cij[C, i] + C_info[2];
                                    open[loop, 3] = cij[C, i] + C_info[2] + h[i];
                                    noloop = false;
                                }
                            }

                            //normal
                            else
                            {
                                if (cij[C, i] == double.MaxValue)
                                { }
                                else
                                {
                                    double[] EX_info = new double[] { i, C, cij[C, i] + C_info[2], cij[C, i] + C_info[2] + h[i] }; //{current point,origin point,G.function,F.function}
                                    m = into(m, EX_info);
                                }
                            }
                        }
                    }
                }

                if ((m.GetLength(0) == 0) && (noloop))// if open can be enlarging and doesn't in loop , C_info add to de)
                {
                    de = into(de, C_info);
                }
                else
                {
                    close = into(close, C_info);
                    for (int i = 0; i < m.GetLength(0); i++)
                    {
                        double[] EX_infor = new double[4];
                        for (int j = 0; j < 4; j++)
                        {
                            EX_infor[j] = m[i, j];
                        }
                        open = into(open, EX_infor);
                    }
                }

                //stop situation
                reach_target = false;
                for (int i = 0; i < open.GetLength(0); i++)
                {
                    reach_target |= (open[i, 0] == tlat.Length - 1);//stop situation is arrive target point
                    if (open[i, 0] == tlat.Length - 1)
                    {
                        abs = open[i, 1];
                        asg = i;
                    }
                }
                astar_g = open[asg, 2];// G.function after through turning point (total travel distance)
            } while (!reach_target);

            // when goal , trace travel can get path
            double[] ans = new double[] { tlat.Length - 1, abs };

            do
            {
                int i_close = 0;
                while (!(close[i_close, 0] == abs))
                {
                    i_close++;
                }
                Array.Resize(ref ans, ans.Length + 1);
                ans[ans.Length - 1] = close[i_close, 1];
                abs = close[i_close, 1];

            } while (!(abs == 0));

            for (int i = 0; i < ans.Length; i++)
            {
                astar_ans[i] = Convert.ToInt32(ans[i]);//
            }
            
            return astar_g;            
        }

        static private double[,] smalldistance(double[] slat, double[] slng)//small distance array for astart program(starting point,turning point,target point)
        {
            double[,] cij = new double[nflatLength + 2, nflatLength + 2];
           
            for (int i = 0; i < nflatLength + 2; i++)
            {
                for (int j = 0; j < nflatLength + 2; j++)
                {
                    bool xx = false;
                    for (int k = 0; k < nflatLength; k++)
                    {
                        xx = xx || crsnofly(slat[i], slng[i], slat[j], slng[j], dll_noflylist[k].Lat, dll_noflylist[k].Lng, dll_noflylist[k + 1].Lat, dll_noflylist[k + 1].Lng);
                    }

                    if (xx == false) cij[i, j] = dist(slat[i], slng[i], slat[j], slng[j]);

                    else cij[i, j] = double.MaxValue;
                }
            }
            return cij;
        }

        static private double[,] delete(double[,] array, int c) //take out and delete the row for astar program delete open.function
        {
            double[,] array1 = new double[array.GetLength(0), array.GetLength(1)];
            Array.Copy(array, array1, array.Length);

            array = new double[array1.GetLength(0) - 1, 4];
            int h = 0;
            for (int i = 0; i < array.GetLength(0) + 1; i++)
            {
                if (i != c)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        array[i - h, j] = array1[i, j];
                    }
                }
                else h++;
            }
            return array;
        }

        static private double[,] into(double[,] array2d, double[] array1d) // add row for astar program open.function,close.function,and dead-end.function 
        {
            double[,] array1 = new double[array2d.GetLength(0), array2d.GetLength(1)];
            Array.Copy(array2d, array1, array2d.Length);

            array2d = new double[array1.GetLength(0) + 1, 4];

            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array2d.GetLength(1); j++)
                {
                    array2d[i, j] = array1[i, j];
                }
            }
            for (int i = 0; i < array2d.GetLength(1); i++)
            {
                array2d[array2d.GetLength(0) - 1, i] = array1d[i];
            }
            return array2d;
        }

        static private bool cw_ccw(double lat0, double lng0, double lat1, double lng1, double lat2, double lng2)//for same direction
        {
            double ccw = (lng0 - lng1) * lat2 + (lat1 - lat0) * lng2 + lat0 * lng1 - lat1 * lng0;
            bool ccw_tf = false;

            if (cw_or_ccw == 0) //cw
            {
                if (ccw > 0) ccw_tf = false;//left
                if (ccw < 0) ccw_tf = true;//right
                if (ccw == 0) ccw_tf = false;//on line
            }
            if (cw_or_ccw == 1) //ccw
            {                
                if (ccw > 0) ccw_tf = true;//left
                if (ccw < 0) ccw_tf = false;//right
                if (ccw == 0) ccw_tf = true;//on line
            }
            return ccw_tf;
        }
    }
}
